using System;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Interfaces;
using CleanArchitectureBase.Application.Interfaces.Services.Identity;
using Microsoft.AspNetCore.SignalR;

namespace CleanArchitectureBase.Application.Hubs
{
    public abstract class HubBase : Hub
    {
        private readonly ISessionProvider _sessionProvider;
        private readonly IUserService _userService;
        private readonly IRoleClaimService _roleService;

        protected HubBase(ISessionProvider sessionProvider, IUserService userService, IRoleClaimService roleService)
        {
            _sessionProvider = sessionProvider;
            _userService = userService;
            _roleService = roleService;
        }

        public override async Task OnConnectedAsync()
        {
            await UpdateGroups(true);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await UpdateGroups(false);
            await base.OnDisconnectedAsync(exception);
        }

        private async Task UpdateGroups(bool add)
        {
            Func<string, Task> fn = add
                ? groupName => Groups.AddToGroupAsync(Context.ConnectionId, groupName)
                : groupName => Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            
            await fn(_sessionProvider.SessionId);

            if (_sessionProvider.UserIdFromSession != null)
            {
                var userRoles = (await _userService.GetRolesAsync(_sessionProvider.UserIdFromSession)).Data.UserRoles.Where(r => r.Selected).ToArray();
                var permissions = userRoles.SelectMany(r => (_roleService.GetAllByRoleIdAsync(r.Id)).Result.Data).Select(r => r.ClaimValue).Distinct();
                
                await fn(_sessionProvider.UserIdFromSession); // For target User
                await Task.WhenAll(permissions.Select(p => fn(p))); // For Target Permissions
                await Task.WhenAll(userRoles.Select(r => fn(r.RoleName))); // For Target Role
            }
        }
    }
}
