using System;
using Volo.Abp.Application.Dtos;

namespace LinkVault.Notifications;

public class AppNotificationDto : EntityDto<Guid>
{
    public string Title { get; set; }
    public string Message { get; set; }
    public string? Url { get; set; }
    public bool IsRead { get; set; }
    public string? Icon { get; set; }
    public DateTime CreationTime { get; set; }
}
