using System;
using System.Threading.Tasks;
using LinkVault.Reminders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;
using Volo.Abp.Uow;

namespace LinkVault.BackgroundJobs;

/// <summary>
/// Background worker that processes due link reminders.
/// Runs periodically to check for reminders that should be triggered.
/// </summary>
public class ReminderNotificationWorker : AsyncPeriodicBackgroundWorkerBase
{
    public ReminderNotificationWorker(
        AbpAsyncTimer timer,
        IServiceScopeFactory serviceScopeFactory)
        : base(timer, serviceScopeFactory)
    {
        // Run every minute
        Timer.Period = 60 * 1000; // 60 seconds
    }

    [UnitOfWork]
    protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
    {
        Logger.LogInformation("ReminderNotificationWorker: Checking for due reminders...");

        var reminderRepository = workerContext.ServiceProvider
            .GetRequiredService<ILinkReminderRepository>();

        // Get due reminders
        var dueReminders = await reminderRepository.GetDueRemindersAsync(100);

        if (dueReminders.Count == 0)
        {
            Logger.LogDebug("ReminderNotificationWorker: No due reminders found.");
            return;
        }

        Logger.LogInformation("ReminderNotificationWorker: Found {Count} due reminders to process.", dueReminders.Count);
        
        // Resolve the notifier service (optional, might be null if not registered yet)
        var notifier = workerContext.ServiceProvider.GetService<IReminderNotifier>();

        foreach (var reminder in dueReminders)
        {
            try
            {
                // Mark as triggered
                reminder.MarkAsTriggered();
                await reminderRepository.UpdateAsync(reminder);

                Logger.LogInformation(
                    "ReminderNotificationWorker: Triggered reminder {ReminderId} for user {UserId}, link: {LinkTitle}",
                    reminder.Id,
                    reminder.UserId,
                    reminder.Link?.Title ?? "Unknown");

                // Create and save persistent notification
                var notificationRepository = workerContext.ServiceProvider.GetService<LinkVault.Notifications.IAppNotificationRepository>();
                var guidGenerator = workerContext.ServiceProvider.GetRequiredService<Volo.Abp.Guids.IGuidGenerator>();
                var currentTenant = workerContext.ServiceProvider.GetRequiredService<Volo.Abp.MultiTenancy.ICurrentTenant>();
                Guid? notificationId = null;

                if (notificationRepository != null)
                {
                    var notification = new LinkVault.Notifications.AppNotification(
                        guidGenerator.Create(),
                        reminder.UserId,
                        reminder.Link?.Title ?? "Link Reminder",
                        $"You have a reminder for: {reminder.Link?.Title}",
                        reminder.Link?.Url,
                        "fas fa-bell", // Default icon
                        currentTenant.Id
                    );
                    
                    await notificationRepository.InsertAsync(notification);
                    notificationId = notification.Id;
                }

                // Send notification if notifier implementation exists
                if (notifier != null)
                {
                    await notifier.NotifyAsync(
                        reminder.UserId, 
                        reminder.Link?.Title ?? "Link Reminder", 
                        $"You have a reminder for: {reminder.Link?.Title}",
                        reminder.LinkId,
                        reminder.Link?.Url,
                        notificationId);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ReminderNotificationWorker: Error processing reminder {ReminderId}", reminder.Id);
            }
        }

        Logger.LogInformation("ReminderNotificationWorker: Finished processing {Count} reminders.", dueReminders.Count);
    }
}
