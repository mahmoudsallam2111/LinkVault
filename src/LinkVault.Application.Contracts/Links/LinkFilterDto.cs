using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace LinkVault.Links;

/// <summary>
/// DTO for filtering links with pagination.
/// </summary>
public class LinkFilterDto : PagedAndSortedResultRequestDto
{
    /// <summary>
    /// Full-text search filter (searches URL, title, description, tags).
    /// </summary>
    public string? Filter { get; set; }

    /// <summary>
    /// Filter by collection ID.
    /// </summary>
    public Guid? CollectionId { get; set; }

    /// <summary>
    /// Filter by tag IDs (links matching ANY of these tags).
    /// </summary>
    public List<Guid>? TagIds { get; set; }

    /// <summary>
    /// Filter by domain (e.g., "github.com").
    /// </summary>
    public string? Domain { get; set; }

    /// <summary>
    /// Filter by favorite status.
    /// </summary>
    public bool? IsFavorite { get; set; }

    /// <summary>
    /// Include deleted links (trash).
    /// </summary>
    public bool IncludeDeleted { get; set; }
}
