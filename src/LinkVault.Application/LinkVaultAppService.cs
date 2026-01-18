using LinkVault.Localization;
using Volo.Abp.Application.Services;

namespace LinkVault;

/* Inherit your application services from this class.
 */
public abstract class LinkVaultAppService : ApplicationService
{
    protected LinkVaultAppService()
    {
        LocalizationResource = typeof(LinkVaultResource);
    }
}
