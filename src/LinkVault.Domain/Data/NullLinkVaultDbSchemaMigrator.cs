using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace LinkVault.Data;

/* This is used if database provider does't define
 * ILinkVaultDbSchemaMigrator implementation.
 */
public class NullLinkVaultDbSchemaMigrator : ILinkVaultDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
