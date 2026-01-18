using LinkVault.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace LinkVault.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class LinkVaultController : AbpControllerBase
{
    protected LinkVaultController()
    {
        LocalizationResource = typeof(LinkVaultResource);
    }
}
