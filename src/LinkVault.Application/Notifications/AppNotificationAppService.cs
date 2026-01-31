using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace LinkVault.Notifications;

[Authorize]
public class AppNotificationAppService : ApplicationService, IAppNotificationAppService
{
    private readonly IAppNotificationRepository _notificationRepository;

    public AppNotificationAppService(IAppNotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<PagedResultDto<AppNotificationDto>> GetListAsync(GetNotificationListDto input)
    {
        if (CurrentUser.Id == null)
        {
            return new PagedResultDto<AppNotificationDto>();
        }

        var notifications = await _notificationRepository.GetListAsync(
            CurrentUser.Id.Value,
            input.UnreadOnly,
            input.SkipCount,
            input.MaxResultCount
        );

        var totalCount = await _notificationRepository.CountAsync(x => x.UserId == CurrentUser.Id.Value && (!input.UnreadOnly || !x.IsRead));

        return new PagedResultDto<AppNotificationDto>(
            totalCount,
            ObjectMapper.Map<List<AppNotification>, List<AppNotificationDto>>(notifications)
        );
    }

    public async Task<long> GetUnreadCountAsync()
    {
        if (CurrentUser.Id == null)
        {
            return 0;
        }

        return await _notificationRepository.GetUnreadCountAsync(CurrentUser.Id.Value);
    }

    public async Task MarkAsReadAsync(Guid id)
    {
        var notification = await _notificationRepository.GetAsync(id);
        
        if (notification.UserId != CurrentUser.Id)
        {
            throw new UnauthorizedAccessException("You can only access your own notifications.");
        }

        notification.MarkAsRead();
        await _notificationRepository.UpdateAsync(notification);
    }

    public async Task MarkAllAsReadAsync()
    {
        if (CurrentUser.Id == null)
        {
            return;
        }

        var unreadNotifications = await _notificationRepository.GetListAsync(
            CurrentUser.Id.Value,
            unreadOnly: true,
            maxResultCount: 1000
        );

        foreach (var notification in unreadNotifications)
        {
            notification.MarkAsRead();
            await _notificationRepository.UpdateAsync(notification);
        }
    }
}
