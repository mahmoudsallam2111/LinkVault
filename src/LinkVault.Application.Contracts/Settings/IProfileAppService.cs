using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace LinkVault.Settings;

public interface IProfileAppService : IApplicationService
{
    Task<UpdateProfileDto> GetProfileAsync();
    Task UpdateProfileAsync(UpdateProfileDto input);
    Task<EmailPreferencesDto> GetEmailPreferencesAsync();
    Task UpdateEmailPreferencesAsync(EmailPreferencesDto input);
    Task ChangePasswordAsync(ChangePasswordDto input);
    Task DeleteAccountAsync(); // Soft delete
}
