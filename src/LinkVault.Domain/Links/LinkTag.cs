using System;
using Volo.Abp.Domain.Entities;
using LinkVault.Tags;

namespace LinkVault.Links;

/// <summary>
/// Junction entity for the many-to-many relationship between Link and Tag.
/// </summary>
public class LinkTag : Entity
{
    /// <summary>
    /// The link ID.
    /// </summary>
    public Guid LinkId { get; private set; }

    /// <summary>
    /// The tag ID.
    /// </summary>
    public Guid TagId { get; private set; }

    /// <summary>
    /// Navigation property to the link.
    /// </summary>
    public virtual Link Link { get; set; } = null!;

    /// <summary>
    /// Navigation property to the tag.
    /// </summary>
    public virtual Tag Tag { get; set; } = null!;

    /// <summary>
    /// Protected constructor for EF Core.
    /// </summary>
    protected LinkTag()
    {
    }

    /// <summary>
    /// Creates a new LinkTag instance.
    /// </summary>
    /// <param name="linkId">The link ID.</param>
    /// <param name="tagId">The tag ID.</param>
    public LinkTag(Guid linkId, Guid tagId)
    {
        LinkId = linkId;
        TagId = tagId;
    }

    /// <summary>
    /// Returns the composite key for this entity.
    /// </summary>
    public override object[] GetKeys()
    {
        return [LinkId, TagId];
    }
}
