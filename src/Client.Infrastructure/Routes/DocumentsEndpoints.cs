namespace CleanArchitectureBase.Client.Infrastructure.Routes
{
    public static class DocumentsEndpoints
    {
        public static string GetAllPaged(int pageNumber, int pageSize, string searchString)
        {
            return $"{BaseEndpoints.Api}/documents?pageNumber={pageNumber}&pageSize={pageSize}&searchString={searchString}";
        }

        public static string GetById(int documentId)
        {
            return $"{BaseEndpoints.Api}/documents/{documentId}";
        }

        public static string Save = $"{BaseEndpoints.Api}/documents";
        public static string Delete = $"{BaseEndpoints.Api}/documents";
    }
}