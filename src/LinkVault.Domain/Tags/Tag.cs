using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using LinkVault.Links;

namespace LinkVault.Tags;

public class Tag : FullAuditedEntity<Guid>
{
   
    public Guid UserId { get; private set; }

    public string Name { get; private set; } = string.Empty;


    public string Color { get; set; } = TagConsts.DefaultColor;

    public virtual ICollection<LinkTag> LinkTags { get; set; } = new List<LinkTag>();

    protected Tag()
    {
    }


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

    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), TagConsts.MaxNameLength);
    }
}
