using System;
using System.Linq;
using System.Threading.Tasks;
using LinkVault.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace LinkVault.Tags;

[AllowAnonymous]
//[Authorize(LinkVaultPermissions.Tags.Default)] -- Test CI
public class TagAppService : ApplicationService, ITagAppService
{
    private readonly ITagRepository _tagRepository;

    public TagAppService(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<TagDto> GetAsync(Guid id)
    {
        var tag = await _tagRepository.GetAsync(id);
        CheckOwnership(tag);
        
        var tagsWithCounts = await _tagRepository.GetWithLinkCountsAsync(tag.UserId);
        var tagWithCount = tagsWithCounts.FirstOrDefault(t => t.Id == id);
        
        return MapToDto(tag, tagWithCount?.LinkCount ?? 0);
    }

    public async Task<ListResultDto<TagDto>> GetListAsync(string? filter = null)
    {
        var userId = CurrentUser.Id!.Value;
        var tagsWithCounts = await _tagRepository.GetWithLinkCountsAsync(userId);

        if (!string.IsNullOrWhiteSpace(filter))
        {
            tagsWithCounts = tagsWithCounts
                .Where(t => t.Name.Contains(filter, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return new ListResultDto<TagDto>(
            tagsWithCounts.Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name,
                Color = t.Color,
                LinkCount = t.LinkCount,
                UserId = userId
            }).ToList());
    }

    //[Authorize(LinkVaultPermissions.Tags.Create)]
    public async Task<TagDto> CreateAsync(CreateUpdateTagDto input)
    {
        var userId = CurrentUser.Id!.Value;

        // Check for duplicate name
        if (await _tagRepository.NameExistsAsync(userId, input.Name))
        {
            throw new BusinessException(LinkVaultDomainErrorCodes.DuplicateTagName)
                .WithData("name", input.Name);
        }

        var tag = new Tag(
            GuidGenerator.Create(),
            userId,
            input.Name)
        {
            Color = input.Color
        };

        await _tagRepository.InsertAsync(tag, autoSave: true);
        return MapToDto(tag, 0);
    }

    // [Authorize(LinkVaultPermissions.Tags.Edit)]
    public async Task<TagDto> UpdateAsync(Guid id, CreateUpdateTagDto input)
    {
        var tag = await _tagRepository.GetAsync(id);
        CheckOwnership(tag);

        // Check for duplicate name
        if (await _tagRepository.NameExistsAsync(tag.UserId, input.Name, id))
        {
            throw new BusinessException(LinkVaultDomainErrorCodes.DuplicateTagName)
                .WithData("name", input.Name);
        }

        tag.SetName(input.Name);
        tag.Color = input.Color;

        await _tagRepository.UpdateAsync(tag, autoSave: true);
        
        var tagsWithCounts = await _tagRepository.GetWithLinkCountsAsync(tag.UserId);
        var tagWithCount = tagsWithCounts.FirstOrDefault(t => t.Id == id);
        
        return MapToDto(tag, tagWithCount?.LinkCount ?? 0);
    }

    //[Authorize(LinkVaultPermissions.Tags.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        var tag = await _tagRepository.GetAsync(id);
        CheckOwnership(tag);
        await _tagRepository.DeleteAsync(tag);
    }

    private void CheckOwnership(Tag tag)
    {
        if (tag.UserId != CurrentUser.Id!.Value)
        {
            throw new BusinessException(LinkVaultDomainErrorCodes.TagNotFound);
        }
    }

    private static TagDto MapToDto(Tag tag, int linkCount)
    {
        return new TagDto
        {
            Id = tag.Id,
            UserId = tag.UserId,
            Name = tag.Name,
            Color = tag.Color,
            LinkCount = linkCount,
            CreationTime = tag.CreationTime,
            LastModificationTime = tag.LastModificationTime
        };
    }
}
