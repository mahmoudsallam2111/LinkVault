using System.Collections.Generic;

namespace LinkVault.Dashboard;

public class DashboardStatsDto
{
    public int TotalLinks { get; set; }
    public int FavoriteCount { get; set; }
    public int TotalClicks { get; set; }

    public int LinksAddedThisWeek { get; set; }

    public int CollectionCount { get; set; }

    public int TagCount { get; set; }

    public Dictionary<string, int> LinksPerCollection { get; set; } = new();
}
