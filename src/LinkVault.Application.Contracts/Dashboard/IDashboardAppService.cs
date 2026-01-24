using System.Collections.Generic;
using System.Threading.Tasks;
using LinkVault.Links.Dtos;
using Volo.Abp.Application.Services;

namespace LinkVault.Dashboard;

/// <summary>
/// Application service interface for dashboard data.
/// </summary>
public interface IDashboardAppService : IApplicationService
{
    /// <summary>
    /// Gets dashboard statistics for the current user.
    /// </summary>
    Task<DashboardStatsDto> GetStatsAsync();

    /// <summary>
    /// Gets the most visited links.
    /// </summary>
    Task<List<LinkDto>> GetMostVisitedAsync(int count = 10);

    /// <summary>
    /// Gets recently added links.
    /// </summary>
    Task<List<LinkDto>> GetRecentAsync(int count = 10);
}
