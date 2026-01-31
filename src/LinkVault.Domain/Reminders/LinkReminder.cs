using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using LinkVault.Links;

namespace LinkVault.Reminders;

/// <summary>
/// Represents a reminder set on a link by a user.
/// When the reminder time is reached, the user receives a notification.
/// </summary>
public class LinkReminder : CreationAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// The user who set this reminder.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// The link this reminder is for.
    /// </summary>
    public Guid LinkId { get; private set; }

    /// <summary>
    /// The date/time when the reminder should trigger.
    /// </summary>
    public DateTime RemindAt { get; private set; }

    /// <summary>
    /// Whether this reminder has been triggered/sent.
    /// </summary>
    public bool IsTriggered { get; private set; }

    /// <summary>
    /// When the reminder was triggered (null if not yet triggered).
    /// </summary>
    public DateTime? TriggeredAt { get; private set; }

    /// <summary>
    /// Optional note to include with the reminder.
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Navigation property to the link.
    /// </summary>
    public virtual Link? Link { get; set; }

    /// <summary>
    /// Protected constructor for EF Core.
    /// </summary>
    protected LinkReminder()
    {
    }

    /// <summary>
    /// Creates a new LinkReminder instance.
    /// </summary>
    public LinkReminder(
        Guid id,
        Guid userId,
        Guid linkId,
        DateTime remindAt,
        string? note = null)
        : base(id)
    {
        Check.NotDefaultOrNull<Guid>(userId, nameof(userId));
        Check.NotDefaultOrNull<Guid>(linkId, nameof(linkId));

        if (remindAt <= DateTime.UtcNow)
        {
            throw new BusinessException(LinkVaultDomainErrorCodes.ReminderTimeMustBeFuture);
        }

        UserId = userId;
        LinkId = linkId;
        RemindAt = remindAt;
        Note = note;
        IsTriggered = false;
    }

    /// <summary>
    /// Marks this reminder as triggered.
    /// </summary>
    public void MarkAsTriggered()
    {
        if (IsTriggered)
        {
            return;
        }

        IsTriggered = true;
        TriggeredAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the reminder time.
    /// </summary>
    public void UpdateRemindAt(DateTime newRemindAt)
    {
        if (newRemindAt <= DateTime.UtcNow)
        {
            throw new BusinessException(LinkVaultDomainErrorCodes.ReminderTimeMustBeFuture);
        }

        RemindAt = newRemindAt;
    }

    /// <summary>
    /// Checks if this reminder is due (should be triggered).
    /// </summary>
    public bool IsDue()
    {
        return !IsTriggered && RemindAt <= DateTime.UtcNow;
    }
}
