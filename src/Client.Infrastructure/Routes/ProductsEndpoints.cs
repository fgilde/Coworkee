using System.Linq;

namespace CleanArchitectureBase.Client.Infrastructure.Routes
{
    public static class ProductsEndpoints
    {
        public static string GetAllPaged(int pageNumber, int pageSize, string searchString, string[] orderBy)
        {
            var url = $"{BaseEndpoints.Api}/products?pageNumber={pageNumber}&pageSize={pageSize}&searchString={searchString}&orderBy=";
            if (orderBy?.Any() == true)
            {
                url = orderBy.Aggregate(url, (current, orderByPart) => current + $"{orderByPart},");
                url = url[..^1]; // loose training ,
            }
            return url;
        }

        public static string GetCount = $"{BaseEndpoints.Api}/products/count";

        public static string GetProductImage(int productId)
        {
            return $"{BaseEndpoints.Api}/products/image/{productId}";
        }

        public static string ExportFiltered(string searchString)
        {
            return $"{Export}?searchString={searchString}";
        }

        public static string Save = $"{BaseEndpoints.Api}/products";
        public static string Delete = $"{BaseEndpoints.Api}/products";
        public static string Export = $"{BaseEndpoints.Api}/products/export";
        public static string ChangePassword = $"{BaseEndpoints.Api}/identity/account/changepassword";
        public static string UpdateProfile = $"{BaseEndpoints.Api}/identity/account/updateprofile";
    }
}