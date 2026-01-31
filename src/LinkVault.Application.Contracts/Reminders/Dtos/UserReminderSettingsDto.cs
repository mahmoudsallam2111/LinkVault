namespace LinkVault.Reminders.Dtos;

/// <summary>
/// DTO for user reminder settings.
/// </summary>
public class UserReminderSettingsDto
{
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
}
