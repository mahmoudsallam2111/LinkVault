using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace LinkVault.Notifications;

public class AppNotification : AuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; protected set; }
    public Guid UserId { get; protected set; }
    public string Title { get; protected set; }
    public string Message { get; protected set; }
    public string? Url { get; protected set; }
    public bool IsRead { get; set; }
    public string? Icon { get; protected set; }

    protected AppNotification()
    {
    }

    public AppNotification(
        Guid id,
        Guid userId,
        string title,
        string message,
        string? url = null,
        string? icon = null,
        Guid? tenantId = null)
        : base(id)
    {
        UserId = userId;
        Title = title;
        Message = message;
        Url = url;
        Icon = icon;
        TenantId = tenantId;
        IsRead = false;
    }

    public void MarkAsRead()
    {
        IsRead = true;
    }
}
