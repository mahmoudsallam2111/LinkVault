using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinkVault.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace LinkVault.Notifications;

public class EfCoreAppNotificationRepository : EfCoreRepository<LinkVaultDbContext, AppNotification, Guid>, IAppNotificationRepository
{
    public EfCoreAppNotificationRepository(IDbContextProvider<LinkVaultDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<AppNotification>> GetListAsync(
        Guid userId,
        bool unreadOnly = false,
        int skipCount = 0,
        int maxResultCount = 10,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = query.Where(x => x.UserId == userId);

        if (unreadOnly)
        {
            query = query.Where(x => !x.IsRead);
        }

        return await query
            .OrderByDescending(x => x.CreationTime)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<long> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        return await query
            .Where(x => x.UserId == userId && !x.IsRead)
            .LongCountAsync(cancellationToken);
    }
}
