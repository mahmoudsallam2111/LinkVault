using System;
using System.Threading.Tasks;

namespace LinkVault.BackgroundJobs;

public interface IReminderNotifier
{
    Task NotifyAsync(Guid userId, string title, string message, Guid? linkId = null, string? url = null, Guid? notificationId = null);
}
