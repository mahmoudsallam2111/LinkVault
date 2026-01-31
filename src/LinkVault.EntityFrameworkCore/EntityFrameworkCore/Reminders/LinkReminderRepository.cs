using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinkVault.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace LinkVault.Reminders;

/// <summary>
/// EF Core implementation of ILinkReminderRepository.
/// </summary>
public class LinkReminderRepository
    : EfCoreRepository<LinkVaultDbContext, LinkReminder, Guid>,
      ILinkReminderRepository
{
    public LinkReminderRepository(IDbContextProvider<LinkVaultDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<LinkReminder>> GetDueRemindersAsync(
        int maxCount = 100,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        var now = DateTime.UtcNow;

        return await dbContext.LinkReminders
            .Include(r => r.Link)
            .Where(r => !r.IsTriggered && r.RemindAt <= now)
            .OrderBy(r => r.RemindAt)
            .Take(maxCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<LinkReminder>> GetPendingByUserAsync(
        Guid userId,
        string? sorting = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();

        var query = dbContext.LinkReminders
            .Include(r => r.Link)
            .Where(r => r.UserId == userId && !r.IsTriggered);

        // Apply sorting
        query = (sorting?.ToLowerInvariant()) switch
        {
            "remindat desc" => query.OrderByDescending(r => r.RemindAt),
            "remindat" or "remindat asc" => query.OrderBy(r => r.RemindAt),
            "creationtime desc" => query.OrderByDescending(r => r.CreationTime),
            _ => query.OrderBy(r => r.RemindAt) // Default: soonest first
        };

        return await query
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetPendingCountByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();

        return await dbContext.LinkReminders
            .CountAsync(r => r.UserId == userId && !r.IsTriggered, cancellationToken);
    }

    public async Task<LinkReminder?> GetPendingByLinkAsync(
        Guid linkId,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();

        return await dbContext.LinkReminders
            .Include(r => r.Link)
            .FirstOrDefaultAsync(r => r.LinkId == linkId && !r.IsTriggered, cancellationToken);
    }

    public async Task<bool> HasPendingReminderAsync(
        Guid linkId,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();

        return await dbContext.LinkReminders
            .AnyAsync(r => r.LinkId == linkId && !r.IsTriggered, cancellationToken);
    }

    public async Task<Dictionary<Guid, LinkReminder>> GetPendingByLinkIdsAsync(
        IEnumerable<Guid> linkIds,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        var linkIdList = linkIds.ToList();

        var reminders = await dbContext.LinkReminders
            .Where(r => linkIdList.Contains(r.LinkId) && !r.IsTriggered)
            .ToListAsync(cancellationToken);

        return reminders.ToDictionary(r => r.LinkId);
    }
}
