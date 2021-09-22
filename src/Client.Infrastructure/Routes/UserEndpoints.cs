namespace CleanArchitectureBase.Client.Infrastructure.Routes
{
    public static class UserEndpoints
    {
        public static string GetAll = $"{BaseEndpoints.Api}/identity/user";

        public static string Get(string userId)
        {
            return $"{BaseEndpoints.Api}/identity/user/{userId}";
        }

        public static string GetUserRoles(string userId)
        {
            return $"{BaseEndpoints.Api}/identity/user/roles/{userId}";
        }

        public static string ExportFiltered(string searchString)
        {
            return $"{Export}?searchString={searchString}";
        }

        public static string Export = $"{BaseEndpoints.Api}/identity/user/export";
        public static string Register = $"{BaseEndpoints.Api}/identity/user";
        public static string ToggleUserStatus = $"{BaseEndpoints.Api}/identity/user/toggle-status";
        public static string ForgotPassword = $"{BaseEndpoints.Api}/identity/user/forgot-password";
        public static string ResetPassword = $"{BaseEndpoints.Api}/identity/user/reset-password";
    }
}