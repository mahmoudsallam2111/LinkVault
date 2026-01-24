using LinkVault.Links.Dtos;
using LinkVault.Permissions;
using LinkVault.Tags;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;

namespace LinkVault.Links;

[AllowAnonymous]
//[Authorize]
public class LinkAppService : ApplicationService, ILinkAppService
{
    private readonly ILinkRepository _linkRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IRepository<LinkTag> _linkTagRepository;
    private readonly LinkManager _linkManager;
    private readonly MetadataFetcherService _metadataFetcher;
    private readonly IDataFilter _dataFilter;

    public LinkAppService(
        ILinkRepository linkRepository,
        ITagRepository tagRepository,
        IRepository<LinkTag> linkTagRepository,
        LinkManager linkManager,
        MetadataFetcherService metadataFetcher,
         IDataFilter dataFilter)
    {
        _linkRepository = linkRepository;
        _tagRepository = tagRepository;
        _linkTagRepository = linkTagRepository;
        _linkManager = linkManager;
        _metadataFetcher = metadataFetcher;
        _dataFilter = dataFilter;
    }

    public async Task<LinkDto> GetAsync(Guid id)
    {
        var link = await _linkRepository.GetAsync(id);
        CheckOwnership(link);
        return MapToDto(link);
    }

    public async Task<PagedResultDto<LinkDto>> GetListAsync(LinkFilterDto input)
    {
        var userId = CurrentUser.Id!.Value;
        
        var totalCount = await _linkRepository.GetCountAsync(
            userId,
            input.Filter,
            input.CollectionId,
            input.TagIds,
            input.Domain,
            input.IsFavorite,
            input.IncludeDeleted);

        var links = await _linkRepository.GetListAsync(
            userId,
            input.Filter,
            input.CollectionId,
            input.TagIds,
            input.Domain,
            input.IsFavorite,
            input.IncludeDeleted,
            input.Sorting,
            input.SkipCount,
            input.MaxResultCount);

        return new PagedResultDto<LinkDto>(
            totalCount,
            links.Select(MapToDto).ToList());
    }

    //[Authorize(LinkVaultPermissions.Links.Create)]
    public async Task<LinkDto> CreateAsync(CreateUpdateLinkDto input)
    {
        var userId = CurrentUser.Id!.Value;

        var link = await _linkManager.CreateAsync(
            userId,
            input.Url,
            input.Title,
            input.Description,
            input.Favicon,
            input.CollectionId);

        link.IsFavorite = input.IsFavorite;

        await _linkRepository.InsertAsync(link, autoSave: true);

        // Handle tags
        if (input.TagNames.Count > 0)
        {
            var tags = await _tagRepository.GetOrCreateByNamesAsync(userId, input.TagNames);
            foreach (var tag in tags)
            {
                await _linkTagRepository.InsertAsync(new LinkTag(link.Id, tag.Id));
            }
        }

        // Reload with relations
        link = await _linkRepository.GetAsync(link.Id);
        return MapToDto(link);
    }

    //[Authorize(LinkVaultPermissions.Links.Create)]
    public async Task<LinkDto> CreateFromUrlAsync(string url, Guid? collectionId = null)
    {
        var userId = CurrentUser.Id!.Value;

        // Fetch metadata
        var metadata = await _metadataFetcher.FetchMetadataAsync(url);
        var title = string.IsNullOrWhiteSpace(metadata.Title) ? url : metadata.Title;

        var link = await _linkManager.CreateAsync(
            userId,
            url,
            title,
            metadata.Description,
            metadata.Favicon,
            collectionId);

        await _linkRepository.InsertAsync(link, autoSave: true);
        return MapToDto(link);
    }

    //[Authorize(LinkVaultPermissions.Links.Edit)]
    public async Task<LinkDto> UpdateAsync(Guid id, CreateUpdateLinkDto input)
    {
        var link = await _linkRepository.GetAsync(id);
        CheckOwnership(link);

        link = await _linkManager.UpdateAsync(
            link,
            input.Url,
            input.Title,
            input.Description,
            input.Favicon,
            input.CollectionId);

        link.IsFavorite = input.IsFavorite;

        await _linkRepository.UpdateAsync(link, autoSave: true);

        // Update tags
        var existingTags = await _linkTagRepository.GetListAsync(lt => lt.LinkId == id);
        await _linkTagRepository.DeleteManyAsync(existingTags);

        if (input.TagNames.Count > 0)
        {
            var tags = await _tagRepository.GetOrCreateByNamesAsync(link.UserId, input.TagNames);
            foreach (var tag in tags)
            {
                await _linkTagRepository.InsertAsync(new LinkTag(link.Id, tag.Id));
            }
        }

        // Reload with relations
        link = await _linkRepository.GetAsync(link.Id);
        return MapToDto(link);
    }

    //[Authorize(LinkVaultPermissions.Links.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        var link = await _linkRepository.GetAsync(id);
        CheckOwnership(link);
        await _linkRepository.DeleteAsync(link); // Soft delete
    }

    //[Authorize(LinkVaultPermissions.Links.Delete)]
    public async Task RestoreAsync(Guid id)
    {
        using (_dataFilter.Disable<ISoftDelete>())
        {
            var link = await _linkRepository.FindAsync(id);
            if (link == null)
            {
                throw new BusinessException(LinkVaultDomainErrorCodes.LinkNotFound);
            }
            CheckOwnership(link);

            link.IsDeleted = false;
            link.DeletionTime = null;
            link.DeleterId = null;

            await _linkRepository.UpdateAsync(link);
        }
    }

    //[Authorize(LinkVaultPermissions.Links.Delete)]
    public async Task HardDeleteAsync(Guid id)
    {
        var link = await _linkRepository.FindAsync(id);
        if (link == null)
        {
            throw new BusinessException(LinkVaultDomainErrorCodes.LinkNotFound);
        }
        CheckOwnership(link);
        await _linkRepository.HardDeleteAsync(link);
    }

    public async Task<LinkDto> ToggleFavoriteAsync(Guid id)
    {
        var link = await _linkRepository.GetAsync(id);
        CheckOwnership(link);
        
        link.ToggleFavorite();
        await _linkRepository.UpdateAsync(link);
        
        return MapToDto(link);
    }

    public async Task<LinkDto> IncrementVisitAsync(Guid id)
    {
        var link = await _linkRepository.GetAsync(id);
        CheckOwnership(link);
        
        link.IncrementVisitCount();
        await _linkRepository.UpdateAsync(link);
        
        return MapToDto(link);
    }

    public async Task<PagedResultDto<LinkDto>> GetTrashAsync(PagedAndSortedResultRequestDto input)
    {
        var userId = CurrentUser.Id!.Value;
        
        var links = await _linkRepository.GetDeletedAsync(
            userId,
            input.Sorting,
            input.SkipCount,
            input.MaxResultCount);

        return new PagedResultDto<LinkDto>(
            links.Count,
            links.Select(MapToDto).ToList());
    }

    public async Task<LinkMetadataDto> FetchMetadataAsync(string url)
    {
        var metadata = await _metadataFetcher.FetchMetadataAsync(url);
        return new LinkMetadataDto
        {
            Title = metadata.Title,
            Description = metadata.Description,
            Favicon = metadata.Favicon
        };
    }

    public async Task<ListResultDto<string>> GetDomainsAsync()
    {
        var userId = CurrentUser.Id!.Value;
        var domains = await _linkRepository.GetDomainsAsync(userId);
        return new ListResultDto<string>(domains);
    }

    public async Task<ListResultDto<LinkDto>> GetAddedThisWeekAsync()
    {
        var userId = CurrentUser.Id!.Value;
        // Using UTC Today as the baseline
        var sinceWeek = DateTime.UtcNow.AddDays(-7);
        var links = await _linkRepository.GetLinksAddedSinceAsync(userId, sinceWeek);
        return new ListResultDto<LinkDto>(links.Select(MapToDto).ToList());
    }

    private void CheckOwnership(Link link)
    {
        if (link.UserId != CurrentUser.Id!.Value)
        {
            throw new BusinessException(LinkVaultDomainErrorCodes.LinkNotFound);
        }
    }

    private LinkDto MapToDto(Link link)
    {
        return new LinkDto
        {
            Id = link.Id,
            UserId = link.UserId,
            Url = link.Url,
            Title = link.Title,
            Description = link.Description,
            Favicon = link.Favicon,
            Domain = link.Domain,
            IsFavorite = link.IsFavorite,
            VisitCount = link.VisitCount,
            CollectionId = link.CollectionId,
            CollectionName = link.Collection?.Name,
            Tags = link.LinkTags?.Select(lt => new LinkTagDto
            {
                Id = lt.Tag.Id,
                Name = lt.Tag.Name,
                Color = lt.Tag.Color
            }).ToList() ?? new List<LinkTagDto>(),
            CreationTime = link.CreationTime,
            LastModificationTime = link.LastModificationTime
        };
    }
}
