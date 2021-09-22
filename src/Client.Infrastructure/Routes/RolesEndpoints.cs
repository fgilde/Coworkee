namespace CleanArchitectureBase.Client.Infrastructure.Routes
{
    public static class RolesEndpoints
    {
        public static string Delete = $"{BaseEndpoints.Api}/identity/role";
        public static string GetAll = $"{BaseEndpoints.Api}/identity/role";
        public static string Save = $"{BaseEndpoints.Api}/identity/role";
        public static string GetPermissions = $"{BaseEndpoints.Api}/identity/role/permissions/";
        public static string UpdatePermissions = $"{BaseEndpoints.Api}/identity/role/permissions/update";
    }
}