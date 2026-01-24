using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LinkVault.Links.Dtos;

public class CreateUpdateLinkDto
{
    [Required]
    [StringLength(LinkConsts.MaxUrlLength)]
    public string Url { get; set; } = string.Empty;

    [Required]
    [StringLength(LinkConsts.MaxTitleLength)]
    public string Title { get; set; } = string.Empty;

    [StringLength(LinkConsts.MaxDescriptionLength)]
    public string? Description { get; set; }

    [StringLength(LinkConsts.MaxFaviconLength)]
    public string? Favicon { get; set; }

    public bool IsFavorite { get; set; }

    public Guid? CollectionId { get; set; }

    public List<string> TagNames { get; set; } = new();
}
