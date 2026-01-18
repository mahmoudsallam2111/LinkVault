using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;
using Volo.Abp.Users;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace LinkVault.Settings;

[Authorize]
public class ProfileAppService : ApplicationService, IProfileAppService
{
    private readonly IdentityUserManager _userManager;
    private readonly IRepository<UserEmailPreferences, Guid> _emailPreferencesRepository;
    private readonly IIdentityUserRepository _userRepository;

    public ProfileAppService(
        IdentityUserManager userManager,
        IRepository<UserEmailPreferences, Guid> emailPreferencesRepository,
        IIdentityUserRepository userRepository)
    {
        _userManager = userManager;
        _emailPreferencesRepository = emailPreferencesRepository;
        _userRepository = userRepository;
    }

    public async Task<UpdateProfileDto> GetProfileAsync()
    {
        var user = await _userManager.GetByIdAsync(CurrentUser.GetId());
        
        return new UpdateProfileDto
        {
            UserName = user.UserName,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email
        };
    }

    public async Task UpdateProfileAsync(UpdateProfileDto input)
    {
        var user = await _userManager.GetByIdAsync(CurrentUser.GetId());

        if (user.UserName != input.UserName)
        {
            await _userManager.SetUserNameAsync(user, input.UserName);
        }

        if (user.Email != input.Email)
        {
            await _userManager.SetEmailAsync(user, input.Email);
        }

        user.Name = input.Name;
        user.Surname = input.Surname;

        await _userManager.UpdateAsync(user);
    }

    public async Task<EmailPreferencesDto> GetEmailPreferencesAsync()
    {
        var userId = CurrentUser.GetId();
        var preferences = await _emailPreferencesRepository.FirstOrDefaultAsync(x => x.UserId == userId);

        if (preferences == null)
        {
            return new EmailPreferencesDto // return default true
            {
                Newsletter = true,
                LinkSharing = true,
                SecurityAlerts = true,
                WeeklyDigest = true
            };
        }

        return ObjectMapper.Map<UserEmailPreferences, EmailPreferencesDto>(preferences);
    }

    public async Task UpdateEmailPreferencesAsync(EmailPreferencesDto input)
    {
        var userId = CurrentUser.GetId();
        var preferences = await _emailPreferencesRepository.FirstOrDefaultAsync(x => x.UserId == userId);

        if (preferences == null)
        {
            preferences = new UserEmailPreferences(GuidGenerator.Create(), userId, input.Newsletter, input.LinkSharing, input.SecurityAlerts, input.WeeklyDigest);
            await _emailPreferencesRepository.InsertAsync(preferences);
        }
        else
        {
            preferences.Newsletter = input.Newsletter;
            preferences.LinkSharing = input.LinkSharing;
            preferences.SecurityAlerts = input.SecurityAlerts;
            preferences.WeeklyDigest = input.WeeklyDigest;
            await _emailPreferencesRepository.UpdateAsync(preferences);
        }
    }

    public async Task ChangePasswordAsync(ChangePasswordDto input)
    {
        var user = await _userManager.GetByIdAsync(CurrentUser.GetId());

        if (!await _userManager.CheckPasswordAsync(user, input.CurrentPassword))
        {
            throw new Volo.Abp.UserFriendlyException("Current password is incorrect.");
        }

        var result = await _userManager.ChangePasswordAsync(user, input.CurrentPassword, input.NewPassword);
        
        if (!result.Succeeded)
        {
            throw new Volo.Abp.UserFriendlyException(string.Join(", ", result.Errors.Select(x => x.Description)));
        }
    }

    public async Task DeleteAccountAsync()
    {
        var user = await _userManager.GetByIdAsync(CurrentUser.GetId());
        // Soft delete is handled by ABP if configured, or we can manually set IsDeleted if we want to ensure it happens immediately here.
        // IdentityUserManager.DeleteAsync usually does a soft delete if the entity implements ISoftDelete (which IdentityUser does).
        await _userManager.DeleteAsync(user);
    }
}
