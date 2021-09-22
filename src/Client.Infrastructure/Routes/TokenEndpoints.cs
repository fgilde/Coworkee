namespace CleanArchitectureBase.Client.Infrastructure.Routes
{
    public static class TokenEndpoints
    {
        public static string Get = $"{BaseEndpoints.Api}/identity/token";
        public static string Refresh = $"{BaseEndpoints.Api}/identity/token/refresh";
    }
}