namespace LinkVault.Permissions;

public static class LinkVaultPermissions
{
    public const string GroupName = "LinkVault";

    public static class Links
    {
        public const string Default = GroupName + ".Links";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Collections
    {
        public const string Default = GroupName + ".Collections";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Tags
    {
        public const string Default = GroupName + ".Tags";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Dashboard
    {
        public const string Default = GroupName + ".Dashboard";
    }

    public static class Import
    {
        public const string Default = GroupName + ".Import";
    }
}
