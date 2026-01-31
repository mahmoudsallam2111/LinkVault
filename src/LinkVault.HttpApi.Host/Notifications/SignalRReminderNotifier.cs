using System;
using System.Threading.Tasks;
using LinkVault.BackgroundJobs;
using LinkVault.Hubs;
using Microsoft.AspNetCore.SignalR;
using Volo.Abp.DependencyInjection;

namespace LinkVault.Notifications;

public class SignalRReminderNotifier : IReminderNotifier, ITransientDependency
{
    private readonly IHubContext<LinkVaultHub> _hubContext;

    public SignalRReminderNotifier(IHubContext<LinkVaultHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyAsync(Guid userId, string title, string message, Guid? linkId = null, string? url = null, Guid? notificationId = null)
    {
        // Send to specific user
        await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveReminder", new 
        { 
            Title = title, 
            Message = message,
            LinkId = linkId,
            Url = url,
            NotificationId = notificationId,
            Time = DateTime.UtcNow
        });
    }
}
