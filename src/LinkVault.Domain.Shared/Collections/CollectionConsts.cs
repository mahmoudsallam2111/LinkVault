namespace LinkVault.Collections;

/// <summary>
/// Constants for Collection entity validation and configuration.
/// </summary>
public static class CollectionConsts
{
    /// <summary>
    /// Maximum length for collection Name field.
    /// </summary>
    public const int MaxNameLength = 128;

    /// <summary>
    /// Maximum length for Color field (hex color code).
    /// </summary>
    public const int MaxColorLength = 7;

    /// <summary>
    /// Maximum length for Icon field (icon class name or emoji).
    /// </summary>
    public const int MaxIconLength = 50;

    /// <summary>
    /// Default color for new collections.
    /// </summary>
    public const string DefaultColor = "#6366F1";
}
