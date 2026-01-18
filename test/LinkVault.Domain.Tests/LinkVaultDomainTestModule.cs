using Volo.Abp.Modularity;

namespace LinkVault;

[DependsOn(
    typeof(LinkVaultDomainModule),
    typeof(LinkVaultTestBaseModule)
)]
public class LinkVaultDomainTestModule : AbpModule
{

}
