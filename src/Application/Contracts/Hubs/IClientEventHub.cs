using System.Threading.Tasks;
using Coworkee.Application.Common.Models.Chat;
using Coworkee.Application.Contracts.Chat;

namespace Coworkee.Application.Contracts.Hubs;

public interface IClientEventHub
{
    Task ConnectUser(string userId);
    Task DisconnectUser(string userId);
    Task UserRolesChanged(string userId);
    Task LogoutUserById(string userId);
    Task ReceiveMessage(ChatHistory<IChatUser> chatHistory, string userName);
    Task UpdateDashboard();
    Task RegenerateTokens();
}