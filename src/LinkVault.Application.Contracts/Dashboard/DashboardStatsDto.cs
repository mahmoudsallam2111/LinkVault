using System.Collections.Generic;

namespace LinkVault.Dashboard;

/// <summary>
/// DTO for dashboard statistics.
/// </summary>
public class DashboardStatsDto
{
    /// <summary>
    /// Total number of saved links.
    /// </summary>
    public int TotalLinks { get; set; }

    /// <summary>
    /// Number of favorite links.
    /// </summary>
    public int FavoriteCount { get; set; }

    /// <summary>
    /// Total number of link clicks/visits.
    /// </summary>
    public int TotalClicks { get; set; }

    /// <summary>
    /// Number of links added in the last 7 days.
    /// </summary>
    public int LinksAddedThisWeek { get; set; }

    /// <summary>
    /// Number of collections.
    /// </summary>
    public int CollectionCount { get; set; }

    /// <summary>
    /// Number of tags.
    /// </summary>
    public int TagCount { get; set; }

    /// <summary>
    /// Links per collection for chart display.
    /// </summary>
    public Dictionary<string, int> LinksPerCollection { get; set; } = new();
}
