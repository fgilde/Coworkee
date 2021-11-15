using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models.Chat;
using CleanArchitectureBase.Application.Contracts;
using CleanArchitectureBase.Application.Contracts.Chat;
using CleanArchitectureBase.Application.Contracts.Services.Identity;
using CleanArchitectureBase.Shared.Constants.Application;
using Microsoft.AspNetCore.SignalR;

namespace CleanArchitectureBase.Application.Hubs
{
    public class ClientEventHub : HubBase
    {
        public ClientEventHub(ISessionProvider sessionProvider, IUserService userService, IRoleClaimService roleService) 
            : base(sessionProvider, userService, roleService)
        { }

        public async Task OnConnectAsync(string userId)
        {
            await Clients.All.SendAsync(ApplicationConstants.SignalR.ConnectUser, userId);
        }

        public async Task OnDisconnectAsync(string userId)
        {
            await Clients.All.SendAsync(ApplicationConstants.SignalR.DisconnectUser, userId);
        }

        public async Task OnChangeRolePermissions(string userId, string roleId)
        {
            await Clients.All.SendAsync(ApplicationConstants.SignalR.LogoutUsersByRole, userId, roleId);
        }

        public async Task SendMessageAsync(ChatHistory<IChatUser> chatHistory, string userName)
        {
            await Clients.All.SendAsync(ApplicationConstants.SignalR.ReceiveMessage, chatHistory, userName);
        }

        public async Task ChatNotificationAsync(string message, string receiverUserId, string senderUserId)
        {
            await Clients.All.SendAsync(ApplicationConstants.SignalR.ReceiveChatNotification, message, receiverUserId, senderUserId);
        }

        public async Task UpdateDashboardAsync()
        {
            await Clients.All.SendAsync(ApplicationConstants.SignalR.ReceiveUpdateDashboard);
        }

        public async Task RegenerateTokensAsync()
        {
            await Clients.All.SendAsync(ApplicationConstants.SignalR.ReceiveRegenerateTokens);
        }
    }
}
