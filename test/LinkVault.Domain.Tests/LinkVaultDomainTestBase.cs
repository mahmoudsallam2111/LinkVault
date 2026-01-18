using Volo.Abp.Modularity;

namespace LinkVault;

/* Inherit from this class for your domain layer tests. */
public abstract class LinkVaultDomainTestBase<TStartupModule> : LinkVaultTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
