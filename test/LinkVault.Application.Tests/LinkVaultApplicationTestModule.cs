using Volo.Abp.Modularity;

namespace LinkVault;

[DependsOn(
    typeof(LinkVaultApplicationModule),
    typeof(LinkVaultDomainTestModule)
)]
public class LinkVaultApplicationTestModule : AbpModule
{

}
