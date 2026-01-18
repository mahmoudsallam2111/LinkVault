using System.Threading.Tasks;

namespace LinkVault.Data;

public interface ILinkVaultDbSchemaMigrator
{
    Task MigrateAsync();
}
