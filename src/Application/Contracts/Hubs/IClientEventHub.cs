using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models.Chat;
using CleanArchitectureBase.Application.Contracts.Chat;

namespace CleanArchitectureBase.Application.Contracts.Hubs;

public interface IClientEventHub
{
    Task ConnectUser(string userId);
    Task DisconnectUser(string userId);
    Task LogoutUsersByRole(string userId, string roleId);
    Task ReceiveMessage(ChatHistory<IChatUser> chatHistory, string userName);
    Task UpdateDashboard();
    Task RegenerateTokens();
}