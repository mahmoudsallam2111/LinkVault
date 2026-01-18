using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace LinkVault.Settings;

public class UserEmailPreferences : AuditedEntity<Guid>
{
    public Guid UserId { get; set; }
    public bool Newsletter { get; set; }
    public bool LinkSharing { get; set; }
    public bool SecurityAlerts { get; set; }
    public bool WeeklyDigest { get; set; }

    protected UserEmailPreferences()
    {
    }

    public UserEmailPreferences(Guid id, Guid userId, bool newsletter = true, bool linkSharing = true, bool securityAlerts = true, bool weeklyDigest = true)
        : base(id)
    {
        UserId = userId;
        Newsletter = newsletter;
        LinkSharing = linkSharing;
        SecurityAlerts = securityAlerts;
        WeeklyDigest = weeklyDigest;
    }
}
