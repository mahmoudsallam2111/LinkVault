using LinkVault.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace LinkVault.Permissions;

public class LinkVaultPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(LinkVaultPermissions.GroupName);

        // Links permissions
        var linksPermission = myGroup.AddPermission(LinkVaultPermissions.Links.Default, L("Permission:Links"));
        linksPermission.AddChild(LinkVaultPermissions.Links.Create, L("Permission:Links.Create"));
        linksPermission.AddChild(LinkVaultPermissions.Links.Edit, L("Permission:Links.Edit"));
        linksPermission.AddChild(LinkVaultPermissions.Links.Delete, L("Permission:Links.Delete"));

        // Collections permissions
        var collectionsPermission = myGroup.AddPermission(LinkVaultPermissions.Collections.Default, L("Permission:Collections"));
        collectionsPermission.AddChild(LinkVaultPermissions.Collections.Create, L("Permission:Collections.Create"));
        collectionsPermission.AddChild(LinkVaultPermissions.Collections.Edit, L("Permission:Collections.Edit"));
        collectionsPermission.AddChild(LinkVaultPermissions.Collections.Delete, L("Permission:Collections.Delete"));

        // Tags permissions
        var tagsPermission = myGroup.AddPermission(LinkVaultPermissions.Tags.Default, L("Permission:Tags"));
        tagsPermission.AddChild(LinkVaultPermissions.Tags.Create, L("Permission:Tags.Create"));
        tagsPermission.AddChild(LinkVaultPermissions.Tags.Edit, L("Permission:Tags.Edit"));
        tagsPermission.AddChild(LinkVaultPermissions.Tags.Delete, L("Permission:Tags.Delete"));

        // Dashboard permission
        myGroup.AddPermission(LinkVaultPermissions.Dashboard.Default, L("Permission:Dashboard"));

        // Import permission
        myGroup.AddPermission(LinkVaultPermissions.Import.Default, L("Permission:Import"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<LinkVaultResource>(name);
    }
}
