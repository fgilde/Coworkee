namespace CleanArchitectureBase.Client.Infrastructure.Routes
{
    public static class AccountEndpoints
    {
        public static string Register = $"{BaseEndpoints.Api}/identity/account/register";
        public static string ChangePassword = $"{BaseEndpoints.Api}/identity/account/changepassword";
        public static string UpdateProfile = $"{BaseEndpoints.Api}/identity/account/updateprofile";

        public static string GetProfilePicture(string userId)
        {
            return $"{BaseEndpoints.Api}/identity/account/profile-picture/{userId}";
        }

        public static string UpdateProfilePicture(string userId)
        {
            return $"{BaseEndpoints.Api}/identity/account/profile-picture/{userId}";
        }
    }
}