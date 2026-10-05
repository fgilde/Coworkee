namespace MyApp.Contracts.Documents;

public static class DocumentPermissions
{
    public const string GroupName = "Documents";

    public static class Documents
    {
        public const string View = "Documents.View";
        public const string Create = "Documents.Create";
        public const string Edit = "Documents.Edit";
        public const string Delete = "Documents.Delete";
        public const string Export = "Documents.Export";
        public const string ManageAll = "Documents.ManageAll";
    }

    public static class Types
    {
        public const string View = "DocumentTypes.View";
        public const string Create = "DocumentTypes.Create";
        public const string Edit = "DocumentTypes.Edit";
        public const string Delete = "DocumentTypes.Delete";
    }
}
