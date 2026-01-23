using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using LinkVault.EntityFrameworkCore;

namespace LinkVault.Collections;

/// <summary>
/// EF Core implementation of ICollectionRepository.
/// </summary>
public class EfCoreCollectionRepository : EfCoreRepository<LinkVaultDbContext, Collection, Guid>, ICollectionRepository
{
    public EfCoreCollectionRepository(IDbContextProvider<LinkVaultDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<Collection>> GetListAsync(
        Guid userId,
        string? filter = null,
        Guid? parentId = null,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();

        var query = dbSet.Where(x => x.UserId == userId);

        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = query.Where(x => x.Name.ToLower().Contains(filter.ToLower()));
        }

        if (parentId.HasValue)
        {
            query = query.Where(x => x.ParentId == parentId.Value);
        }

        return await query
            .OrderBy(x => x.Order)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Collection>> GetTreeAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();

        return await dbSet
            .Where(x => x.UserId == userId && x.ParentId == null)
            .Include(x => x.Children.OrderBy(c => c.Order).ThenBy(c => c.Name))
                .ThenInclude(x => x.Children.OrderBy(c => c.Order).ThenBy(c => c.Name))
            .OrderBy(x => x.Order)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> NameExistsAsync(
        Guid userId,
        string name,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();

        var query = dbSet
            .Where(x => x.UserId == userId)
            .Where(x => x.Name.ToLower() == name.ToLower());

        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<Dictionary<Guid, int>> GetLinkCountsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();

        return await dbContext.Collections
            .Where(c => c.UserId == userId)
            .Select(c => new
            {
                c.Id,
                Count = dbContext.Links.Count(l => l.CollectionId == c.Id && !l.IsDeleted)
            })
            .ToDictionaryAsync(x => x.Id, x => x.Count, cancellationToken);
    }

    public async Task<Collection?> FindByShareTokenAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();

        return await dbSet
            .Include(c => c.Links.Where(l => !l.IsDeleted))
            .FirstOrDefaultAsync(x => x.PublicShareToken == token, cancellationToken);
    }
}
