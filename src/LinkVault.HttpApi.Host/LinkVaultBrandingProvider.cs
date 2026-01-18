using Microsoft.Extensions.Localization;
using LinkVault.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace LinkVault;

[Dependency(ReplaceServices = true)]
public class LinkVaultBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<LinkVaultResource> _localizer;

    public LinkVaultBrandingProvider(IStringLocalizer<LinkVaultResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
