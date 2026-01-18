using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;
using LinkVault.Links;
using LinkVault.Collections;
using LinkVault.Tags;

namespace LinkVault;

/* Link Mappers */
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class LinkToLinkDtoMapper : MapperBase<Link, LinkDto>
{
    public override partial LinkDto Map(Link source);

    public override partial void Map(Link source, LinkDto destination);
}

/* Collection Mappers */
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CollectionToCollectionDtoMapper : MapperBase<Collection, CollectionDto>
{
    public override partial CollectionDto Map(Collection source);

    public override partial void Map(Collection source, CollectionDto destination);
}

/* Tag Mappers */
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class TagToTagDtoMapper : MapperBase<Tag, TagDto>
{
    public override partial TagDto Map(Tag source);

    public override partial void Map(Tag source, TagDto destination);
}
