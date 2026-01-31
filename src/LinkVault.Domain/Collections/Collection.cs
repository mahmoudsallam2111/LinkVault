using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace LinkVault.Collections;
public class Collection : FullAuditedAggregateRoot<Guid>
{
    public Guid UserId { get; private set; }

    public Guid? ParentId { get; set; }
    public string Name { get; private set; } = string.Empty;

    public string Color { get; set; } = CollectionConsts.DefaultColor;

    public string? Icon { get; set; }

    public int Order { get; set; }
    public string? PublicShareToken { get; private set; }

    public virtual Collection? Parent { get; set; }

    public virtual ICollection<Collection> Children { get; set; } = new List<Collection>();
    public virtual ICollection<Links.Link> Links { get; set; } = new List<Links.Link>();
    protected Collection()
    {
    }

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

    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), CollectionConsts.MaxNameLength);
    }

    public string GenerateShareToken()
    {
        PublicShareToken = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N")[..8];
        return PublicShareToken;
    }
    public void RevokeShareToken()
    {
        PublicShareToken = null;
    }
}
