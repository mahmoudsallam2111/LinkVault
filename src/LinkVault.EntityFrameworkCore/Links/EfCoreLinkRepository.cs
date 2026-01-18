using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using LinkVault.EntityFrameworkCore;

namespace LinkVault.Links;

/// <summary>
/// EF Core implementation of ILinkRepository.
/// </summary>
public class EfCoreLinkRepository : EfCoreRepository<LinkVaultDbContext, Link, Guid>, ILinkRepository
{
    public EfCoreLinkRepository(IDbContextProvider<LinkVaultDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<Link>> GetListAsync(
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
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var query = ApplyFilters(dbSet, userId, filter, collectionId, tagIds, domain, isFavorite, includeDeleted);

        query = query
            .OrderBy(string.IsNullOrWhiteSpace(sorting) ? "CreationTime DESC" : sorting)
            .Skip(skipCount)
            .Take(maxResultCount);

        return await query
            .Include(x => x.Collection)
            .Include(x => x.LinkTags)
                .ThenInclude(x => x.Tag)
            .ToListAsync(cancellationToken);
    }

    public async Task<long> GetCountAsync(
        Guid userId,
        string? filter = null,
        Guid? collectionId = null,
        List<Guid>? tagIds = null,
        string? domain = null,
        bool? isFavorite = null,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var query = ApplyFilters(dbSet, userId, filter, collectionId, tagIds, domain, isFavorite, includeDeleted);

        return await query.LongCountAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Guid userId,
        string url,
        Guid? excludeLinkId = null,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var normalizedUrl = url.ToLowerInvariant().TrimEnd('/');

        var query = dbSet
            .Where(x => x.UserId == userId)
            .Where(x => x.Url.ToLower() == normalizedUrl || x.Url.ToLower() == normalizedUrl + "/");

        if (excludeLinkId.HasValue)
        {
            query = query.Where(x => x.Id != excludeLinkId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<List<Link>> GetMostVisitedAsync(
        Guid userId,
        int count = 10,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();

        return await dbSet
            .Where(x => x.UserId == userId && !x.IsDeleted)
            .OrderByDescending(x => x.VisitCount)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Link>> GetRecentAsync(
        Guid userId,
        int count = 10,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();

        return await dbSet
            .Where(x => x.UserId == userId && !x.IsDeleted)
            .OrderByDescending(x => x.CreationTime)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Link>> GetDeletedAsync(
        Guid userId,
        string? sorting = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();

        return await dbContext.Links
            .IgnoreQueryFilters()
            .Where(x => x.UserId == userId && x.IsDeleted)
            .OrderBy(string.IsNullOrWhiteSpace(sorting) ? "DeletionTime DESC" : sorting)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<string>> GetDomainsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();

        return await dbSet
            .Where(x => x.UserId == userId && !x.IsDeleted)
            .Select(x => x.Domain)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Link>> GetLinksAddedSinceAsync(
        Guid userId,
        DateTime startTime,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();

        return await dbSet
            .Where(x => x.UserId == userId && !x.IsDeleted && x.CreationTime >= startTime)
            .OrderByDescending(x => x.CreationTime)
            .Include(x => x.Collection)
            .Include(x => x.LinkTags)
                .ThenInclude(x => x.Tag)
            .ToListAsync(cancellationToken);
    }

    public async Task<LinkStatsDto> GetStatsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var dbContext = await GetDbContextAsync();

        var stats = new LinkStatsDto();

        var linksQuery = dbSet.Where(x => x.UserId == userId && !x.IsDeleted);

        stats.TotalLinks = await linksQuery.CountAsync(cancellationToken);
        stats.FavoriteCount = await linksQuery.CountAsync(x => x.IsFavorite, cancellationToken);
        stats.TotalClicks = await linksQuery.SumAsync(x => x.VisitCount, cancellationToken);

        var weekAgo = DateTime.UtcNow.AddDays(-7);
        stats.LinksAddedThisWeek = await linksQuery
            .CountAsync(x => x.CreationTime >= weekAgo, cancellationToken);

        // Fetch collection names and their IDs first to handle grouping in memory if needed, 
        // or purely in database. GroupBy in EF Core transliteration can be tricky with complex projections.
        // We will fetch the raw data and group in memory to be safe and avoid complex EF Core translation issues
        // or multiple round-trips for names.
        
        var collectionStats = await dbContext.Collections
            .Where(c => c.UserId == userId && !c.IsDeleted)
            .Select(c => new 
            { 
                c.Id, 
                c.Name 
            })
            .ToListAsync(cancellationToken);

        var collectionIds = collectionStats.Select(x => x.Id).ToList();
        
        // Get counts for these collections
        var linkCounts = await dbSet
            .Where(l => l.CollectionId.HasValue && collectionIds.Contains(l.CollectionId.Value) && !l.IsDeleted)
            .GroupBy(l => l.CollectionId)
            .Select(g => new { CollectionId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        // Join and Group by Name in memory
        stats.LinksPerCollection = collectionStats
            .GroupJoin(
                linkCounts,
                c => c.Id,
                l => l.CollectionId,
                (c, l) => new { c.Name, Count = l.Sum(x => x.Count) } // l is empty or single
            )
            .GroupBy(x => x.Name)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(x => x.Count)
            );

        return stats;
    }

    private static IQueryable<Link> ApplyFilters(
        DbSet<Link> dbSet,
        Guid userId,
        string? filter,
        Guid? collectionId,
        List<Guid>? tagIds,
        string? domain,
        bool? isFavorite,
        bool includeDeleted)
    {
        IQueryable<Link> query = dbSet;

        if (includeDeleted)
        {
            query = dbSet.IgnoreQueryFilters();
        }

        query = query.Where(x => x.UserId == userId);

        if (!includeDeleted)
        {
            query = query.Where(x => !x.IsDeleted);
        }

        if (!string.IsNullOrWhiteSpace(filter))
        {
            var filterLower = filter.ToLower();
            query = query.Where(x =>
                x.Title.ToLower().Contains(filterLower) ||
                x.Url.ToLower().Contains(filterLower) ||
                (x.Description != null && x.Description.ToLower().Contains(filterLower)) ||
                x.LinkTags.Any(lt => lt.Tag.Name.ToLower().Contains(filterLower)));
        }

        if (collectionId.HasValue)
        {
            query = query.Where(x => x.CollectionId == collectionId.Value);
        }

        if (tagIds != null && tagIds.Count > 0)
        {
            query = query.Where(x => x.LinkTags.Any(lt => tagIds.Contains(lt.TagId)));
        }

        if (!string.IsNullOrWhiteSpace(domain))
        {
            query = query.Where(x => x.Domain == domain.ToLower());
        }

        if (isFavorite.HasValue)
        {
            query = query.Where(x => x.IsFavorite == isFavorite.Value);
        }

        return query;
    }
}
