using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using LinkVault.Links;

namespace LinkVault.Tags;

/// <summary>
/// Represents a tag for categorizing links.
/// Tags have a many-to-many relationship with links.
/// </summary>
public class Tag : FullAuditedEntity<Guid>
{
    /// <summary>
    /// The user who owns this tag.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// The name of the tag.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// The color of the tag (hex color code).
    /// </summary>
    public string Color { get; set; } = TagConsts.DefaultColor;

    /// <summary>
    /// Navigation property to links with this tag.
    /// </summary>
    public virtual ICollection<LinkTag> LinkTags { get; set; } = new List<LinkTag>();

    /// <summary>
    /// Protected constructor for EF Core.
    /// </summary>
    protected Tag()
    {
    }

    /// <summary>
    /// Creates a new Tag instance.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="userId">The owner user ID.</param>
    /// <param name="name">The name of the tag.</param>
    public Tag(
        Guid id,
        Guid userId,
        string name)
        : base(id)
    {
        UserId = userId;
        SetName(name);
        Color = TagConsts.DefaultColor;
    }

    /// <summary>
    /// Updates the name of the tag.
    /// </summary>
    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), TagConsts.MaxNameLength);
    }
}
