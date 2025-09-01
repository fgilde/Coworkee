using System.Threading.Tasks;
using Coworkee.Application.Contracts;
using Coworkee.Application.Contracts.Hubs;
using Coworkee.Application.Contracts.Services.Identity;

namespace Coworkee.Application.Hubs
{
    public class ClientEventHub : HubBase<IClientEventHub>
    {
        public ClientEventHub(ISessionProvider sessionProvider, IUserService userService, IRoleClaimService roleService) 
            : base(sessionProvider, userService, roleService)
        { }

        public async Task OnConnectAsync(string userId)
        {
            await JoinGroups(userId);
            await Clients.All.ConnectUser(userId);
        }

        public async Task OnDisconnectAsync(string userId)
        {
            await LeaveGroups(userId);
            await Clients.All.DisconnectUser(userId);
        }

        public async Task UserRolesChanged(string userId)
        {
            await Clients.All.UserRolesChanged(userId);
        }

        public async Task LogoutUserById(string userId)
        {
            await Clients.All.LogoutUserById(userId);
        }

        public async Task UpdateDashboardAsync()
        {
            await Clients.All.UpdateDashboard();
        }

        public async Task RegenerateTokensAsync()
        {
            await Clients.All.RegenerateTokens();
        }
    }
}
