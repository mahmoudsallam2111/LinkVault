namespace LinkVault.Tags;

/// <summary>
/// Constants for Tag entity validation and configuration.
/// </summary>
public static class TagConsts
{
    /// <summary>
    /// Maximum length for tag Name field.
    /// </summary>
    public const int MaxNameLength = 64;

    /// <summary>
    /// Maximum length for Color field (hex color code).
    /// </summary>
    public const int MaxColorLength = 7;

    /// <summary>
    /// Default color for new tags.
    /// </summary>
    public const string DefaultColor = "#8B5CF6";
}
