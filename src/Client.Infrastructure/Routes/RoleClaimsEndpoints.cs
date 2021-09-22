namespace CleanArchitectureBase.Client.Infrastructure.Routes
{
    class RoleClaimsEndpoints
    {
        public static string Delete = $"{BaseEndpoints.Api}/identity/roleClaim";
        public static string GetAll = $"{BaseEndpoints.Api}/identity/roleClaim";
        public static string Save = $"{BaseEndpoints.Api}/identity/roleClaim";
    }
}