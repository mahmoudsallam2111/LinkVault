using System;
using System.ComponentModel.DataAnnotations;

namespace LinkVault.Reminders.Dtos;

/// <summary>
/// DTO for creating a link reminder.
/// </summary>
public class CreateLinkReminderDto
{
    /// <summary>
    /// The link to set a reminder for.
    /// </summary>
    [Required]
    public Guid LinkId { get; set; }

    /// <summary>
    /// When to remind (UTC). Either this or DurationHours must be set.
    /// </summary>
    public DateTime? RemindAt { get; set; }

    /// <summary>
    /// Duration from now in hours. Either this or RemindAt must be set.
    /// </summary>
    public int? DurationHours { get; set; }

    /// <summary>
    /// Optional note to include with the reminder.
    /// </summary>
    [MaxLength(500)]
    public string? Note { get; set; }
}
