using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkVault.Links;
using LinkVault.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace LinkVault.Collections;

[AllowAnonymous]
//[Authorize(LinkVaultPermissions.Collections.Default)]
public class CollectionAppService : ApplicationService, ICollectionAppService
{
    private readonly ICollectionRepository _collectionRepository;

    public CollectionAppService(ICollectionRepository collectionRepository)
    {
        _collectionRepository = collectionRepository;
    }

    public async Task<CollectionDto> GetAsync(Guid id)
    {
        var collection = await _collectionRepository.GetAsync(id);
        CheckOwnership(collection);
        
        var linkCounts = await _collectionRepository.GetLinkCountsAsync(collection.UserId);
        return MapToDto(collection, linkCounts);
    }

    public async Task<ListResultDto<CollectionDto>> GetListAsync(string? filter = null)
    {
        var userId = CurrentUser.Id!.Value;
        var collections = await _collectionRepository.GetListAsync(userId, filter);
        var linkCounts = await _collectionRepository.GetLinkCountsAsync(userId);

        return new ListResultDto<CollectionDto>(
            collections.Select(c => MapToDto(c, linkCounts)).ToList());
    }

    public async Task<ListResultDto<CollectionDto>> GetTreeAsync()
    {
        var userId = CurrentUser.Id!.Value;
        var collections = await _collectionRepository.GetTreeAsync(userId);
        var linkCounts = await _collectionRepository.GetLinkCountsAsync(userId);

        return new ListResultDto<CollectionDto>(
            collections.Select(c => MapToTreeDto(c, linkCounts)).ToList());
    }

   // [Authorize(LinkVaultPermissions.Collections.Create)]
    public async Task<CollectionDto> CreateAsync(CreateUpdateCollectionDto input)
    {
        var userId = CurrentUser.Id!.Value;

        // Check for duplicate name
        if (await _collectionRepository.NameExistsAsync(userId, input.Name))
        {
            throw new BusinessException(LinkVaultDomainErrorCodes.DuplicateCollectionName)
                .WithData("name", input.Name);
        }

        var collection = new Collection(
            GuidGenerator.Create(),
            userId,
            input.Name,
            input.ParentId)
        {
            Color = input.Color,
            Icon = input.Icon,
            Order = input.Order
        };

        await _collectionRepository.InsertAsync(collection, autoSave: true);
        
        var linkCounts = await _collectionRepository.GetLinkCountsAsync(userId);
        return MapToDto(collection, linkCounts);
    }

    //[Authorize(LinkVaultPermissions.Collections.Edit)]
    public async Task<CollectionDto> UpdateAsync(Guid id, CreateUpdateCollectionDto input)
    {
        var collection = await _collectionRepository.GetAsync(id);
        CheckOwnership(collection);

        // Check for duplicate name
        if (await _collectionRepository.NameExistsAsync(collection.UserId, input.Name, id))
        {
            throw new BusinessException(LinkVaultDomainErrorCodes.DuplicateCollectionName)
                .WithData("name", input.Name);
        }

        collection.SetName(input.Name);
        collection.Color = input.Color;
        collection.Icon = input.Icon;
        collection.Order = input.Order;

        if (input.ParentId != collection.ParentId)
        {
            // Validate no circular reference
            if (input.ParentId.HasValue && await WouldCreateCircularReference(id, input.ParentId.Value))
            {
                throw new BusinessException(LinkVaultDomainErrorCodes.CircularCollectionReference);
            }
            collection.ParentId = input.ParentId;
        }

        await _collectionRepository.UpdateAsync(collection, autoSave: true);
        
        var linkCounts = await _collectionRepository.GetLinkCountsAsync(collection.UserId);
        return MapToDto(collection, linkCounts);
    }

   // [Authorize(LinkVaultPermissions.Collections.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        var collection = await _collectionRepository.GetAsync(id);
        CheckOwnership(collection);
        await _collectionRepository.DeleteAsync(collection);
    }

    //[Authorize(LinkVaultPermissions.Collections.Edit)]
    public async Task<CollectionDto> MoveAsync(Guid id, Guid? newParentId)
    {
        var collection = await _collectionRepository.GetAsync(id);
        CheckOwnership(collection);

        if (newParentId.HasValue && await WouldCreateCircularReference(id, newParentId.Value))
        {
            throw new BusinessException(LinkVaultDomainErrorCodes.CircularCollectionReference);
        }

        collection.ParentId = newParentId;
        await _collectionRepository.UpdateAsync(collection, autoSave: true);
        
        var linkCounts = await _collectionRepository.GetLinkCountsAsync(collection.UserId);
        return MapToDto(collection, linkCounts);
    }

  //  [Authorize(LinkVaultPermissions.Collections.Edit)]
    public async Task ReorderAsync(List<ReorderCollectionDto> items)
    {
        var userId = CurrentUser.Id!.Value;

        foreach (var item in items)
        {
            var collection = await _collectionRepository.GetAsync(item.Id);
            if (collection.UserId != userId) continue;
            
            collection.Order = item.Order;
            await _collectionRepository.UpdateAsync(collection);
        }
    }

    public async Task<CollectionDto> GenerateShareTokenAsync(Guid id)
    {
        var collection = await _collectionRepository.GetAsync(id);
        CheckOwnership(collection);

        // Generate new token if not already shared
        if (string.IsNullOrEmpty(collection.PublicShareToken))
        {
            collection.GenerateShareToken();
            await _collectionRepository.UpdateAsync(collection, autoSave: true);
        }

        var linkCounts = await _collectionRepository.GetLinkCountsAsync(collection.UserId);
        return MapToDto(collection, linkCounts);
    }

    public async Task RevokeShareTokenAsync(Guid id)
    {
        var collection = await _collectionRepository.GetAsync(id);
        CheckOwnership(collection);

        collection.RevokeShareToken();
        await _collectionRepository.UpdateAsync(collection, autoSave: true);
    }

    [AllowAnonymous]
    public async Task<PublicCollectionDto?> GetByShareTokenAsync(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return null;
        }

        var collection = await _collectionRepository.FindByShareTokenAsync(token);
        if (collection == null)
        {
            return null;
        }

        return new PublicCollectionDto
        {
            Id = collection.Id,
            Name = collection.Name,
            Color = collection.Color,
            Icon = collection.Icon,
            LinkCount = collection.Links?.Count ?? 0,
            Links = collection.Links?.Select(l => new PublicLinkDto
            {
                Id = l.Id,
                Title = l.Title,
                Url = l.Url,
                Description = l.Description,
                FaviconUrl = l.Favicon
            }).ToList() ?? new List<PublicLinkDto>()
        };
    }

    private async Task<bool> WouldCreateCircularReference(Guid collectionId, Guid newParentId)
    {
        if (collectionId == newParentId) return true;

        var parent = await _collectionRepository.FindAsync(newParentId);
        while (parent != null)
        {
            if (parent.Id == collectionId) return true;
            if (!parent.ParentId.HasValue) break;
            parent = await _collectionRepository.FindAsync(parent.ParentId.Value);
        }

        return false;
    }

    private void CheckOwnership(Collection collection)
    {
        if (collection.UserId != CurrentUser.Id!.Value)
        {
            throw new BusinessException(LinkVaultDomainErrorCodes.CollectionNotFound);
        }
    }

    private static CollectionDto MapToDto(Collection collection, Dictionary<Guid, int> linkCounts)
    {
        return new CollectionDto
        {
            Id = collection.Id,
            UserId = collection.UserId,
            ParentId = collection.ParentId,
            Name = collection.Name,
            Color = collection.Color,
            Icon = collection.Icon,
            Order = collection.Order,
            LinkCount = linkCounts.GetValueOrDefault(collection.Id, 0),
            PublicShareToken = collection.PublicShareToken,
            CreationTime = collection.CreationTime,
            LastModificationTime = collection.LastModificationTime
        };
    }

    private static CollectionDto MapToTreeDto(Collection collection, Dictionary<Guid, int> linkCounts)
    {
        var dto = MapToDto(collection, linkCounts);
        dto.Children = collection.Children?
            .Select(c => MapToTreeDto(c, linkCounts))
            .ToList() ?? new List<CollectionDto>();
        return dto;
    }
}
