using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace LinkVault.Settings;

/// <summary>
/// User-specific settings for reminders.
/// </summary>
public class UserReminderSettings : AuditedEntity<Guid>
{
    /// <summary>
    /// The user these settings belong to.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Default reminder duration in hours.
    /// </summary>
    public int DefaultReminderHours { get; set; }

    /// <summary>
    /// Whether to receive in-app notifications for reminders.
    /// </summary>
    public bool EnableInAppNotifications { get; set; }

    /// <summary>
    /// Whether to receive email notifications for reminders.
    /// </summary>
    public bool EnableEmailNotifications { get; set; }

    protected UserReminderSettings()
    {
    }

    public UserReminderSettings(
        Guid id,
        Guid userId,
        int defaultReminderHours = 24,
        bool enableInAppNotifications = true,
        bool enableEmailNotifications = false)
        : base(id)
    {
        UserId = userId;
        DefaultReminderHours = defaultReminderHours;
        EnableInAppNotifications = enableInAppNotifications;
        EnableEmailNotifications = enableEmailNotifications;
    }
}
