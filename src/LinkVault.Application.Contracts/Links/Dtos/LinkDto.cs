using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace LinkVault.Links.Dtos;

public class LinkDto : FullAuditedEntityDto<Guid>
{
    public Guid UserId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Favicon { get; set; }
    public string Domain { get; set; } = string.Empty;
    public bool IsFavorite { get; set; }
    public int VisitCount { get; set; }
    public Guid? CollectionId { get; set; }
    public string? CollectionName { get; set; }
    public List<LinkTagDto> Tags { get; set; } = new();
}
