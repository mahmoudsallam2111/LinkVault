namespace LinkVault;

public static class LinkVaultDomainErrorCodes
{
    /* Link error codes */
    public const string DuplicateUrl = "LinkVault:DuplicateUrl";
    public const string LinkNotFound = "LinkVault:LinkNotFound";
    public const string InvalidUrl = "LinkVault:InvalidUrl";

    /* Collection error codes */
    public const string DuplicateCollectionName = "LinkVault:DuplicateCollectionName";
    public const string CollectionNotFound = "LinkVault:CollectionNotFound";
    public const string CircularCollectionReference = "LinkVault:CircularCollectionReference";

    /* Tag error codes */
    public const string DuplicateTagName = "LinkVault:DuplicateTagName";
    public const string TagNotFound = "LinkVault:TagNotFound";

    /* Import error codes */
    public const string InvalidImportFormat = "LinkVault:InvalidImportFormat";
    public const string ImportFailed = "LinkVault:ImportFailed";

    /* Reminder error codes */
    public const string ReminderAlreadyExists = "LinkVault:ReminderAlreadyExists";
    public const string ReminderNotFound = "LinkVault:ReminderNotFound";
    public const string ReminderTimeMustBeFuture = "LinkVault:ReminderTimeMustBeFuture";
}
