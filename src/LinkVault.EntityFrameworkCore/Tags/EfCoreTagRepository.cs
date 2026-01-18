using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Guids;
using LinkVault.EntityFrameworkCore;

namespace LinkVault.Tags;

/// <summary>
/// EF Core implementation of ITagRepository.
/// </summary>
public class EfCoreTagRepository : EfCoreRepository<LinkVaultDbContext, Tag, Guid>, ITagRepository
{
    private readonly IGuidGenerator _guidGenerator;

    public EfCoreTagRepository(
        IDbContextProvider<LinkVaultDbContext> dbContextProvider,
        IGuidGenerator guidGenerator)
        : base(dbContextProvider)
    {
        _guidGenerator = guidGenerator;
    }

    public async Task<List<Tag>> GetListAsync(
        Guid userId,
        string? filter = null,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();

        var query = dbSet.Where(x => x.UserId == userId);

        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = query.Where(x => x.Name.ToLower().Contains(filter.ToLower()));
        }

        return await query
            .OrderBy(x => x.Name)
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

    public async Task<List<TagWithCountDto>> GetWithLinkCountsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();

        return await dbContext.Tags
            .Where(t => t.UserId == userId)
            .Select(t => new TagWithCountDto
            {
                Id = t.Id,
                Name = t.Name,
                Color = t.Color,
                LinkCount = t.LinkTags.Count(lt => !lt.Link.IsDeleted)
            })
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Tag>> GetOrCreateByNamesAsync(
        Guid userId,
        List<string> names,
        CancellationToken cancellationToken = default)
    {
        if (names == null || names.Count == 0)
        {
            return new List<Tag>();
        }

        var dbSet = await GetDbSetAsync();
        var dbContext = await GetDbContextAsync();
        var normalizedNames = names.Select(n => n.Trim().ToLower()).Distinct().ToList();

        // Get existing tags
        var existingTags = await dbSet
            .Where(t => t.UserId == userId && normalizedNames.Contains(t.Name.ToLower()))
            .ToListAsync(cancellationToken);

        var existingNames = existingTags.Select(t => t.Name.ToLower()).ToHashSet();
        var result = new List<Tag>(existingTags);

        // Create missing tags
        foreach (var name in names.Where(n => !existingNames.Contains(n.Trim().ToLower())))
        {
            var tag = new Tag(_guidGenerator.Create(), userId, name.Trim());
            await dbSet.AddAsync(tag, cancellationToken);
            result.Add(tag);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return result;
    }
}
