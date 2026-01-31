using System;
using System.Linq;
using System.Threading.Tasks;
using LinkVault.Links;
using LinkVault.Reminders.Dtos;
using LinkVault.Settings;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace LinkVault.Reminders;

/// <summary>
/// Application service for managing link reminders.
/// </summary>
[AllowAnonymous]
public class LinkReminderAppService : ApplicationService, ILinkReminderAppService
{
    private readonly ILinkReminderRepository _reminderRepository;
    private readonly ILinkRepository _linkRepository;
    private readonly IRepository<UserReminderSettings, Guid> _settingsRepository;

    public LinkReminderAppService(
        ILinkReminderRepository reminderRepository,
        ILinkRepository linkRepository,
        IRepository<UserReminderSettings, Guid> settingsRepository)
    {
        _reminderRepository = reminderRepository;
        _linkRepository = linkRepository;
        _settingsRepository = settingsRepository;
    }

    public async Task<LinkReminderDto> CreateAsync(CreateLinkReminderDto input)
    {
        var userId = CurrentUser.Id!.Value;

        // Verify link ownership
        var link = await _linkRepository.GetAsync(input.LinkId);
        if (link.UserId != userId)
        {
            throw new BusinessException(LinkVaultDomainErrorCodes.LinkNotFound);
        }

        // Check if reminder already exists for this link
        if (await _reminderRepository.HasPendingReminderAsync(input.LinkId))
        {
            throw new BusinessException(LinkVaultDomainErrorCodes.ReminderAlreadyExists);
        }

        // Calculate remind time
        DateTime remindAt;
        if (input.RemindAt.HasValue)
        {
            remindAt = input.RemindAt.Value.ToUniversalTime();
        }
        else if (input.DurationHours.HasValue)
        {
            remindAt = DateTime.UtcNow.AddHours(input.DurationHours.Value);
        }
        else
        {
            // Use user's default duration
            var settings = await GetOrCreateSettingsAsync(userId);
            remindAt = DateTime.UtcNow.AddHours(settings.DefaultReminderHours);
        }

        var reminder = new LinkReminder(
            GuidGenerator.Create(),
            userId,
            input.LinkId,
            remindAt,
            input.Note);

        await _reminderRepository.InsertAsync(reminder, autoSave: true);

        // Reload with link details
        reminder = await _reminderRepository.GetAsync(reminder.Id);
        return MapToDto(reminder);
    }

    public async Task DeleteAsync(Guid id)
    {
        var reminder = await _reminderRepository.GetAsync(id);
        if (reminder.UserId != CurrentUser.Id!.Value)
        {
            throw new BusinessException(LinkVaultDomainErrorCodes.ReminderNotFound);
        }

        await _reminderRepository.DeleteAsync(reminder);
    }

    public async Task<PagedResultDto<LinkReminderDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var userId = CurrentUser.Id!.Value;

        var count = await _reminderRepository.GetPendingCountByUserAsync(userId);
        var reminders = await _reminderRepository.GetPendingByUserAsync(
            userId,
            input.Sorting,
            input.SkipCount,
            input.MaxResultCount);

        return new PagedResultDto<LinkReminderDto>(
            count,
            reminders.Select(MapToDto).ToList());
    }

    public async Task<int> GetPendingCountAsync()
    {
        var userId = CurrentUser.Id!.Value;
        return await _reminderRepository.GetPendingCountByUserAsync(userId);
    }

    public async Task<LinkReminderDto?> GetByLinkAsync(Guid linkId)
    {
        var reminder = await _reminderRepository.GetPendingByLinkAsync(linkId);
        if (reminder == null)
        {
            return null;
        }

        if (reminder.UserId != CurrentUser.Id!.Value)
        {
            return null;
        }

        return MapToDto(reminder);
    }

    public async Task<UserReminderSettingsDto> GetSettingsAsync()
    {
        var userId = CurrentUser.Id!.Value;
        var settings = await GetOrCreateSettingsAsync(userId);
        return new UserReminderSettingsDto
        {
            DefaultReminderHours = settings.DefaultReminderHours,
            EnableInAppNotifications = settings.EnableInAppNotifications,
            EnableEmailNotifications = settings.EnableEmailNotifications
        };
    }

    public async Task<UserReminderSettingsDto> UpdateSettingsAsync(UserReminderSettingsDto input)
    {
        var userId = CurrentUser.Id!.Value;
        var settings = await GetOrCreateSettingsAsync(userId);

        settings.DefaultReminderHours = input.DefaultReminderHours;
        settings.EnableInAppNotifications = input.EnableInAppNotifications;
        settings.EnableEmailNotifications = input.EnableEmailNotifications;

        await _settingsRepository.UpdateAsync(settings);

        return input;
    }

    private async Task<UserReminderSettings> GetOrCreateSettingsAsync(Guid userId)
    {
        var settings = await _settingsRepository.FirstOrDefaultAsync(s => s.UserId == userId);
        if (settings == null)
        {
            settings = new UserReminderSettings(
                GuidGenerator.Create(),
                userId,
                ReminderConsts.DefaultReminderHours);
            await _settingsRepository.InsertAsync(settings, autoSave: true);
        }
        return settings;
    }

    private LinkReminderDto MapToDto(LinkReminder reminder)
    {
        return new LinkReminderDto
        {
            Id = reminder.Id,
            UserId = reminder.UserId,
            LinkId = reminder.LinkId,
            RemindAt = reminder.RemindAt,
            IsTriggered = reminder.IsTriggered,
            TriggeredAt = reminder.TriggeredAt,
            Note = reminder.Note,
            LinkTitle = reminder.Link?.Title ?? string.Empty,
            LinkUrl = reminder.Link?.Url ?? string.Empty,
            LinkFavicon = reminder.Link?.Favicon,
            LinkDomain = reminder.Link?.Domain ?? string.Empty,
            CreationTime = reminder.CreationTime
        };
    }
}
