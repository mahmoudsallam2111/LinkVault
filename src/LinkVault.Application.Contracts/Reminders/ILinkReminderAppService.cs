using System;
using System.Threading.Tasks;
using LinkVault.Reminders.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace LinkVault.Reminders;

/// <summary>
/// Application service interface for link reminders.
/// </summary>
public interface ILinkReminderAppService : IApplicationService
{
    /// <summary>
    /// Creates a new reminder for a link.
    /// </summary>
    Task<LinkReminderDto> CreateAsync(CreateLinkReminderDto input);

    /// <summary>
    /// Deletes a reminder.
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Gets a list of pending reminders for the current user.
    /// </summary>
    Task<PagedResultDto<LinkReminderDto>> GetListAsync(PagedAndSortedResultRequestDto input);

    /// <summary>
    /// Gets the count of pending reminders for the current user.
    /// </summary>
    Task<int> GetPendingCountAsync();

    /// <summary>
    /// Gets the pending reminder for a specific link (if any).
    /// </summary>
    Task<LinkReminderDto?> GetByLinkAsync(Guid linkId);

    /// <summary>
    /// Gets the user's reminder settings.
    /// </summary>
    Task<UserReminderSettingsDto> GetSettingsAsync();

    /// <summary>
    /// Updates the user's reminder settings.
    /// </summary>
    Task<UserReminderSettingsDto> UpdateSettingsAsync(UserReminderSettingsDto input);
}
