using Volo.Abp.Application.Dtos;

namespace LinkVault.Notifications;

public class GetNotificationListDto : PagedResultRequestDto
{
    public bool UnreadOnly { get; set; }
}
