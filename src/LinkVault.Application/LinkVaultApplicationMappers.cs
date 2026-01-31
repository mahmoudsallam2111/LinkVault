using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;
using LinkVault.Links;
using LinkVault.Collections;
using LinkVault.Tags;
using LinkVault.Settings;
using LinkVault.Links.Dtos;

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

/* UserEmailPreferences Mappers */
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class UserEmailPreferencesToEmailPreferencesDtoMapper : MapperBase<UserEmailPreferences, EmailPreferencesDto>
{
    public override partial EmailPreferencesDto Map(UserEmailPreferences source);

    public override partial void Map(UserEmailPreferences source, EmailPreferencesDto destination);
}

/* AppNotification Mappers */
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class AppNotificationToAppNotificationDtoMapper : MapperBase<Notifications.AppNotification, Notifications.AppNotificationDto>
{
    public override partial Notifications.AppNotificationDto Map(Notifications.AppNotification source);

    public override partial void Map(Notifications.AppNotification source, Notifications.AppNotificationDto destination);
}
