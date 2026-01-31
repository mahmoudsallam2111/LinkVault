using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace LinkVault.Notifications;

public interface IAppNotificationAppService : IApplicationService
{
    Task<PagedResultDto<AppNotificationDto>> GetListAsync(GetNotificationListDto input);
    Task<long> GetUnreadCountAsync();
    Task MarkAsReadAsync(Guid id);
    Task MarkAllAsReadAsync();
}
