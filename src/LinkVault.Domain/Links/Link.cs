using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using LinkVault.Collections;

namespace LinkVault.Links;

public class Link : FullAuditedAggregateRoot<Guid>
{
    public Guid UserId { get; private set; }

    public string Url { get; private set; } = string.Empty;

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public string? Favicon { get; set; }

    public string Domain { get; private set; } = string.Empty;

    public bool IsFavorite { get; set; }

    public int VisitCount { get; private set; }

    public Guid? CollectionId { get; set; }

    public virtual Collection? Collection { get; set; }

    public virtual ICollection<LinkTag> LinkTags { get; set; } = new List<LinkTag>();

    /// <summary>
    /// Protected constructor for EF Core.
    /// </summary>
    protected Link()
    {
    }

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

    public void SetUrl(string url)
    {
        Url = Check.NotNullOrWhiteSpace(url, nameof(url), LinkConsts.MaxUrlLength);
    }
    public void IncrementVisitCount()
    {
        VisitCount++;
    }
    public void ToggleFavorite()
    {
        IsFavorite = !IsFavorite;
    }
}
