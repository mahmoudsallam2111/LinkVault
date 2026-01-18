using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace LinkVault.Links;

/// <summary>
/// Custom repository interface for Link entity with advanced querying capabilities.
/// </summary>
public interface ILinkRepository : IRepository<Link, Guid>
{
    /// <summary>
    /// Gets a list of links with filtering, sorting, and pagination.
    /// </summary>
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

    /// <summary>
    /// Gets the count of links matching the filter criteria.
    /// </summary>
    Task<long> GetCountAsync(
        Guid userId,
        string? filter = null,
        Guid? collectionId = null,
        List<Guid>? tagIds = null,
        string? domain = null,
        bool? isFavorite = null,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a URL already exists for a user.
    /// </summary>
    Task<bool> ExistsAsync(
        Guid userId,
        string url,
        Guid? excludeLinkId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the most visited links for a user.
    /// </summary>
    Task<List<Link>> GetMostVisitedAsync(
        Guid userId,
        int count = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets recently added links for a user.
    /// </summary>
    Task<List<Link>> GetRecentAsync(
        Guid userId,
        int count = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets deleted links (trash) for a user.
    /// </summary>
    Task<List<Link>> GetDeletedAsync(
        Guid userId,
        string? sorting = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all unique domains for a user's links.
    /// </summary>
    Task<List<string>> GetDomainsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets links added since a specific date for a user.
    /// </summary>
    Task<List<Link>> GetLinksAddedSinceAsync(
        Guid userId,
        DateTime startTime,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets link statistics for dashboard.
    /// </summary>
    Task<LinkStatsDto> GetStatsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// DTO for link statistics.
/// </summary>
public class LinkStatsDto
{
    public int TotalLinks { get; set; }
    public int FavoriteCount { get; set; }
    public int TotalClicks { get; set; }
    public int LinksAddedThisWeek { get; set; }
    public Dictionary<string, int> LinksPerCollection { get; set; } = new();
}
