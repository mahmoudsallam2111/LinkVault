using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace LinkVault.Collections;

/// <summary>
/// DTO for displaying collection information.
/// </summary>
public class CollectionDto : FullAuditedEntityDto<Guid>
{
    public Guid UserId { get; set; }
    public Guid? ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public int Order { get; set; }
    public int LinkCount { get; set; }
    public List<CollectionDto> Children { get; set; } = new();
}
