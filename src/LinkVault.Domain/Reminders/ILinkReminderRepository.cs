using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace LinkVault.Reminders;

/// <summary>
/// Repository interface for LinkReminder entity.
/// </summary>
public interface ILinkReminderRepository : IRepository<LinkReminder, Guid>
{
    /// <summary>
    /// Gets all reminders that are due (past their remind time and not yet triggered).
    /// </summary>
    Task<List<LinkReminder>> GetDueRemindersAsync(
        int maxCount = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all pending (not triggered) reminders for a user.
    /// </summary>
    Task<List<LinkReminder>> GetPendingByUserAsync(
        Guid userId,
        string? sorting = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of pending reminders for a user.
    /// </summary>
    Task<int> GetPendingCountByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the pending reminder for a specific link (if any).
    /// </summary>
    Task<LinkReminder?> GetPendingByLinkAsync(
        Guid linkId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a link has a pending reminder.
    /// </summary>
    Task<bool> HasPendingReminderAsync(
        Guid linkId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets reminders by link IDs (for batch checking).
    /// </summary>
    Task<Dictionary<Guid, LinkReminder>> GetPendingByLinkIdsAsync(
        IEnumerable<Guid> linkIds,
        CancellationToken cancellationToken = default);
}
