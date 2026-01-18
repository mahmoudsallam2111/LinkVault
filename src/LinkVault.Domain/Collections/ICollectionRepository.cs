using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace LinkVault.Collections;

/// <summary>
/// Custom repository interface for Collection entity.
/// </summary>
public interface ICollectionRepository : IRepository<Collection, Guid>
{
    /// <summary>
    /// Gets all collections for a user as a flat list.
    /// </summary>
    Task<List<Collection>> GetListAsync(
        Guid userId,
        string? filter = null,
        Guid? parentId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets collections for a user as a tree structure.
    /// </summary>
    Task<List<Collection>> GetTreeAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a collection name already exists for a user.
    /// </summary>
    Task<bool> NameExistsAsync(
        Guid userId,
        string name,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the link count for each collection.
    /// </summary>
    Task<Dictionary<Guid, int>> GetLinkCountsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
