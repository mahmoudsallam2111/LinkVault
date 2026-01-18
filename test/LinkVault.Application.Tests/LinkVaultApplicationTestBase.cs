using Volo.Abp.Modularity;

namespace LinkVault;

public abstract class LinkVaultApplicationTestBase<TStartupModule> : LinkVaultTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
