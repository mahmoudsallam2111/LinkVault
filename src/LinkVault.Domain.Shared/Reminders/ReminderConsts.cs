namespace LinkVault.Reminders;

/// <summary>
/// Constants for reminder validation and defaults.
/// </summary>
public static class ReminderConsts
{
    /// <summary>
    /// Minimum reminder duration in minutes.
    /// </summary>
    public const int MinReminderMinutes = 5;

    /// <summary>
    /// Maximum reminder duration in days.
    /// </summary>
    public const int MaxReminderDays = 365;

    /// <summary>
    /// Default reminder durations in hours.
    /// </summary>
    public static class DefaultDurations
    {
        public const int OneHour = 1;
        public const int ThreeHours = 3;
        public const int OneDay = 24;
        public const int ThreeDays = 72;
        public const int OneWeek = 168;
    }

    /// <summary>
    /// Default reminder duration for new users (in hours).
    /// </summary>
    public const int DefaultReminderHours = 24;
}
