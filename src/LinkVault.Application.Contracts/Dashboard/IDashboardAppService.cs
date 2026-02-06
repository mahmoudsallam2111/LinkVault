using System.Collections.Generic;
using System.Threading.Tasks;
using LinkVault.Links.Dtos;
using Volo.Abp.Application.Services;

namespace LinkVault.Dashboard;

public interface IDashboardAppService : IApplicationService
{
    Task<DashboardStatsDto> GetStatsAsync();

    Task<List<LinkDto>> GetMostVisitedAsync(int count = 10);

    Task<List<LinkDto>> GetRecentAsync(int count = 10);
}
