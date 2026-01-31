using System;
using Volo.Abp.Domain.Entities;
using LinkVault.Tags;

namespace LinkVault.Links;

public class LinkTag : Entity
{
    public Guid LinkId { get; private set; }

    public Guid TagId { get; private set; }

    public virtual Link Link { get; set; } = null!;

    public virtual Tag Tag { get; set; } = null!;

    protected LinkTag()
    {
    }

    public LinkTag(Guid linkId, Guid tagId)
    {
        LinkId = linkId;
        TagId = tagId;
    }

    public override object[] GetKeys()
    {
        return [LinkId, TagId];
    }
}
