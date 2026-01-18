using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace LinkVault.Collections;

/// <summary>
/// Represents a collection (folder) for organizing links.
/// Supports nested collections with parent/child relationships.
/// </summary>
public class Collection : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// The user who owns this collection.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// The parent collection ID for nested collections.
    /// Null for root-level collections.
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// The name of the collection.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// The color of the collection (hex color code).
    /// </summary>
    public string Color { get; set; } = CollectionConsts.DefaultColor;

    /// <summary>
    /// The icon for the collection (icon class name or emoji).
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// The display order of the collection.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Navigation property to the parent collection.
    /// </summary>
    public virtual Collection? Parent { get; set; }

    /// <summary>
    /// Navigation property to child collections.
    /// </summary>
    public virtual ICollection<Collection> Children { get; set; } = new List<Collection>();

    /// <summary>
    /// Navigation property to links in this collection.
    /// </summary>
    public virtual ICollection<Links.Link> Links { get; set; } = new List<Links.Link>();

    /// <summary>
    /// Protected constructor for EF Core.
    /// </summary>
    protected Collection()
    {
    }

    /// <summary>
    /// Creates a new Collection instance.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="userId">The owner user ID.</param>
    /// <param name="name">The name of the collection.</param>
    /// <param name="parentId">The parent collection ID (optional).</param>
    public Collection(
        Guid id,
        Guid userId,
        string name,
        Guid? parentId = null)
        : base(id)
    {
        UserId = userId;
        SetName(name);
        ParentId = parentId;
        Color = CollectionConsts.DefaultColor;
        Order = 0;
    }

    /// <summary>
    /// Updates the name of the collection.
    /// </summary>
    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), CollectionConsts.MaxNameLength);
    }
}
