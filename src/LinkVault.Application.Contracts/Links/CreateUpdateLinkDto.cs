using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LinkVault.Links;

/// <summary>
/// DTO for creating or updating a link.
/// </summary>
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

    /// <summary>
    /// List of tag names to associate with the link.
    /// Tags will be created if they don't exist.
    /// </summary>
    public List<string> TagNames { get; set; } = new();
}
