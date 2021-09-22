namespace CleanArchitectureBase.Client.Infrastructure.Routes
{
    public static class BrandsEndpoints
    {
        public static string ExportFiltered(string searchString)
        {
            return $"{Export}?searchString={searchString}";
        }

        public static string Export = $"{BaseEndpoints.Api}/brands/export";

        public static string GetAll = $"{BaseEndpoints.Api}/brands";
        public static string Delete = $"{BaseEndpoints.Api}/brands";
        public static string Save = $"{BaseEndpoints.Api}/brands";
        public static string GetCount = $"{BaseEndpoints.Api}/brands/count";
    }
}