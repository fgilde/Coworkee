using System.Threading.Tasks;
using lib.Coworkee.Application.Common.Models.Chat;
using lib.Coworkee.Application.Contracts.Chat;

namespace lib.Coworkee.Application.Contracts.Hubs;

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