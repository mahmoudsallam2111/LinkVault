using System;
using Volo.Abp.Application.Dtos;

namespace LinkVault.Reminders.Dtos;

/// <summary>
/// DTO for displaying a link reminder.
/// </summary>
public class LinkReminderDto : CreationAuditedEntityDto<Guid>
{
    public Guid UserId { get; set; }
    public Guid LinkId { get; set; }
    public DateTime RemindAt { get; set; }
    public bool IsTriggered { get; set; }
    public DateTime? TriggeredAt { get; set; }
    public string? Note { get; set; }

    // Link details for display
    public string LinkTitle { get; set; } = string.Empty;
    public string LinkUrl { get; set; } = string.Empty;
    public string? LinkFavicon { get; set; }
    public string LinkDomain { get; set; } = string.Empty;
}
