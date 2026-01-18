using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using LinkVault.Collections;

namespace LinkVault.Links;

/// <summary>
/// Represents a saved link/bookmark in the system.
/// This is the main aggregate root for the link management bounded context.
/// </summary>
public class Link : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// The user who owns this link.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// The URL of the link.
    /// </summary>
    public string Url { get; private set; } = string.Empty;

    /// <summary>
    /// The title of the link (usually fetched from page metadata).
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Description of the link (usually fetched from page metadata).
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// URL to the favicon of the linked page.
    /// </summary>
    public string? Favicon { get; set; }

    /// <summary>
    /// The domain extracted from the URL (e.g., "github.com").
    /// </summary>
    public string Domain { get; private set; } = string.Empty;

    /// <summary>
    /// Whether this link is marked as a favorite.
    /// </summary>
    public bool IsFavorite { get; set; }

    /// <summary>
    /// Number of times this link has been visited/clicked.
    /// </summary>
    public int VisitCount { get; private set; }

    /// <summary>
    /// The collection this link belongs to (optional).
    /// </summary>
    public Guid? CollectionId { get; set; }

    /// <summary>
    /// Navigation property to the collection.
    /// </summary>
    public virtual Collection? Collection { get; set; }

    /// <summary>
    /// Tags associated with this link.
    /// </summary>
    public virtual ICollection<LinkTag> LinkTags { get; set; } = new List<LinkTag>();

    /// <summary>
    /// Protected constructor for EF Core.
    /// </summary>
    protected Link()
    {
    }

    /// <summary>
    /// Creates a new Link instance.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="userId">The owner user ID.</param>
    /// <param name="url">The URL of the link.</param>
    /// <param name="title">The title of the link.</param>
    /// <param name="domain">The domain extracted from the URL.</param>
    public Link(
        Guid id,
        Guid userId,
        string url,
        string title,
        string domain)
        : base(id)
    {
        UserId = userId;
        SetUrl(url);
        Title = Check.NotNullOrWhiteSpace(title, nameof(title), LinkConsts.MaxTitleLength);
        Domain = Check.NotNullOrWhiteSpace(domain, nameof(domain), LinkConsts.MaxDomainLength);
        VisitCount = 0;
        IsFavorite = false;
    }

    /// <summary>
    /// Updates the URL of the link.
    /// </summary>
    public void SetUrl(string url)
    {
        Url = Check.NotNullOrWhiteSpace(url, nameof(url), LinkConsts.MaxUrlLength);
    }

    /// <summary>
    /// Increments the visit count for this link.
    /// </summary>
    public void IncrementVisitCount()
    {
        VisitCount++;
    }

    /// <summary>
    /// Toggles the favorite status.
    /// </summary>
    public void ToggleFavorite()
    {
        IsFavorite = !IsFavorite;
    }
}
