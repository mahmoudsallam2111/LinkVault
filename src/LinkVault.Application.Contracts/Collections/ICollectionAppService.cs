using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace LinkVault.Collections;

/// <summary>
/// Application service interface for managing collections.
/// </summary>
public interface ICollectionAppService : IApplicationService
{
    /// <summary>
    /// Gets a collection by ID.
    /// </summary>
    Task<CollectionDto> GetAsync(Guid id);

    /// <summary>
    /// Gets all collections as a flat list.
    /// </summary>
    Task<ListResultDto<CollectionDto>> GetListAsync(string? filter = null);

    /// <summary>
    /// Gets collections as a nested tree structure.
    /// </summary>
    Task<ListResultDto<CollectionDto>> GetTreeAsync();

    /// <summary>
    /// Creates a new collection.
    /// </summary>
    Task<CollectionDto> CreateAsync(CreateUpdateCollectionDto input);

    /// <summary>
    /// Updates an existing collection.
    /// </summary>
    Task<CollectionDto> UpdateAsync(Guid id, CreateUpdateCollectionDto input);

    /// <summary>
    /// Deletes a collection.
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Moves a collection to a different parent.
    /// </summary>
    Task<CollectionDto> MoveAsync(Guid id, Guid? newParentId);

    /// <summary>
    /// Updates the order of multiple collections.
    /// </summary>
    Task ReorderAsync(List<ReorderCollectionDto> items);
}

/// <summary>
/// DTO for reordering collections.
/// </summary>
public class ReorderCollectionDto
{
    public Guid Id { get; set; }
    public int Order { get; set; }
}
