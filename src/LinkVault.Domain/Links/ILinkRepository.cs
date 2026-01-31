using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace LinkVault.Links;

public interface ILinkRepository : IRepository<Link, Guid>
{
    Task<List<Link>> GetListAsync(
        Guid userId,
        string? filter = null,
        Guid? collectionId = null,
        List<Guid>? tagIds = null,
        string? domain = null,
        bool? isFavorite = null,
        bool includeDeleted = false,
        string? sorting = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
        Guid userId,
        string? filter = null,
        Guid? collectionId = null,
        List<Guid>? tagIds = null,
        string? domain = null,
        bool? isFavorite = null,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Guid userId,
        string url,
        Guid? excludeLinkId = null,
        CancellationToken cancellationToken = default);

    Task<List<Link>> GetMostVisitedAsync(
        Guid userId,
        int count = 10,
        CancellationToken cancellationToken = default);
    Task<List<Link>> GetRecentAsync(
        Guid userId,
        int count = 10,
        CancellationToken cancellationToken = default);

    Task<List<Link>> GetDeletedAsync(
        Guid userId,
        string? sorting = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        CancellationToken cancellationToken = default);

    Task<List<string>> GetDomainsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<List<Link>> GetLinksAddedSinceAsync(
        Guid userId,
        DateTime startTime,
        CancellationToken cancellationToken = default);
    Task<LinkStatsDto> GetStatsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}

public class LinkStatsDto
{
    public int TotalLinks { get; set; }
    public int FavoriteCount { get; set; }
    public int TotalClicks { get; set; }
    public int LinksAddedThisWeek { get; set; }
    public Dictionary<string, int> LinksPerCollection { get; set; } = new();
}
