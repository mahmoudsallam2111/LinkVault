using LinkVault.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace LinkVault.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(LinkVaultEntityFrameworkCoreModule),
    typeof(LinkVaultApplicationContractsModule)
)]
public class LinkVaultDbMigratorModule : AbpModule
{
}
