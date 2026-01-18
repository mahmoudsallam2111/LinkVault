
export interface DashboardStatsDto {
  totalLinks?: number;
  favoriteCount?: number;
  totalClicks?: number;
  linksAddedThisWeek?: number;
  collectionCount?: number;
  tagCount?: number;
  linksPerCollection?: Record<string, number>;
}
