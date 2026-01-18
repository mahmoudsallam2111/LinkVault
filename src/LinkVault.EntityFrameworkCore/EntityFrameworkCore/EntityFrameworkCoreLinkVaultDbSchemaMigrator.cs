using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LinkVault.Data;
using Volo.Abp.DependencyInjection;

namespace LinkVault.EntityFrameworkCore;

public class EntityFrameworkCoreLinkVaultDbSchemaMigrator
    : ILinkVaultDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreLinkVaultDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the LinkVaultDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<LinkVaultDbContext>()
            .Database
            .MigrateAsync();
    }
}
