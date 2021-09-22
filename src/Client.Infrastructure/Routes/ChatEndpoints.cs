namespace CleanArchitectureBase.Client.Infrastructure.Routes
{
    public static class ChatEndpoint
    {
        public static string GetAvailableUsers = $"{BaseEndpoints.Api}/chats/users";
        public static string SaveMessage = $"{BaseEndpoints.Api}/chats";

        public static string GetChatHistory(string userId)
        {
            return $"{BaseEndpoints.Api}/chats/{userId}";
        }
    }
}