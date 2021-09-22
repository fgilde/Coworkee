using CleanArchitectureBase.Application.Features.Documents.Queries.GetAll;
using CleanArchitectureBase.Client.Infrastructure.Extensions;

namespace CleanArchitectureBase.Client.Infrastructure.Routes
{
    public static class DocumentsEndpoints
    {
        public static string GetAllPaged(GetAllDocumentsQuery query)
        {
            return $"{BaseEndpoints.Api}/documents/{query.AsGet()}";
        }

        public static string GetById(int documentId)
        {
            return $"{BaseEndpoints.Api}/documents/{documentId}";
        }

        public static string Save = $"{BaseEndpoints.Api}/documents";
        public static string Delete = $"{BaseEndpoints.Api}/documents";
    }
}