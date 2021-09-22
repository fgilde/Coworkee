using System.Linq;
using CleanArchitectureBase.Application.Features.Products.Queries.GetAllPaged;
using CleanArchitectureBase.Client.Infrastructure.Extensions;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Client.Infrastructure.Routes
{
    public static class ProductsEndpoints
    {
        public static string GetAllPaged(GetAllProductsQuery query)
        {
            return $"{BaseEndpoints.Api}/products/{query.AsGet()}";
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