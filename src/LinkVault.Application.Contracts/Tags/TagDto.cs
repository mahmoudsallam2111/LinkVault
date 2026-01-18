using System;
using Volo.Abp.Application.Dtos;

namespace LinkVault.Tags;

/// <summary>
/// DTO for displaying tag information.
/// </summary>
public class TagDto : FullAuditedEntityDto<Guid>
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int LinkCount { get; set; }
}
