namespace CleanArchitectureBase.Client.Infrastructure.Routes
{
    public static class DocumentTypesEndpoints
    {
        public static string ExportFiltered(string searchString)
        {
            return $"{Export}?searchString={searchString}";
        }

        public static string Export = $"{BaseEndpoints.Api}/documentTypes/export";

        public static string GetAll = $"{BaseEndpoints.Api}/documentTypes";
        public static string Delete = $"{BaseEndpoints.Api}/documentTypes";
        public static string Save = $"{BaseEndpoints.Api}/documentTypes";
        public static string GetCount = $"{BaseEndpoints.Api}/documentTypes/count";
    }
}