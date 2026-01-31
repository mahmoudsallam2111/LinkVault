using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace LinkVault.Notifications;

public interface IAppNotificationRepository : IRepository<AppNotification, Guid>
{
    Task<List<AppNotification>> GetListAsync(
        Guid userId,
        bool unreadOnly = false,
        int skipCount = 0,
        int maxResultCount = 10,
        CancellationToken cancellationToken = default);

    Task<long> GetUnreadCountAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
