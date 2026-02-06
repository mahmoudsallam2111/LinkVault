using System;
using System.Threading.Tasks;
using LinkVault.Reminders.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace LinkVault.Reminders;

public interface ILinkReminderAppService : IApplicationService
{

    Task<LinkReminderDto> CreateAsync(CreateLinkReminderDto input);

    Task DeleteAsync(Guid id);

    Task<PagedResultDto<LinkReminderDto>> GetListAsync(PagedAndSortedResultRequestDto input);

    Task<int> GetPendingCountAsync();

    Task<LinkReminderDto?> GetByLinkAsync(Guid linkId);

    Task<UserReminderSettingsDto> GetSettingsAsync();
    Task<UserReminderSettingsDto> UpdateSettingsAsync(UserReminderSettingsDto input);
}
