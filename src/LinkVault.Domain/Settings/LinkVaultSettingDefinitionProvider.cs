using Volo.Abp.Settings;

namespace LinkVault.Settings;

public class LinkVaultSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(LinkVaultSettings.MySetting1));
    }
}
