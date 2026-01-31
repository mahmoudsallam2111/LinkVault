using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace LinkVault.Collections;

public interface ICollectionRepository : IRepository<Collection, Guid>
{

    Task<List<Collection>> GetListAsync(
        Guid userId,
        string? filter = null,
        Guid? parentId = null,
        CancellationToken cancellationToken = default);
    Task<List<Collection>> GetTreeAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<bool> NameExistsAsync(
        Guid userId,
        string name,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, int>> GetLinkCountsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<Collection?> FindByShareTokenAsync(
        string token,
        CancellationToken cancellationToken = default);
}
