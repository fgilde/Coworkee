using System;
using System.Linq;
using System.Threading.Tasks;
using lib.Coworkee.Application.Contracts;
using lib.Coworkee.Application.Contracts.Services.Identity;
using Microsoft.AspNetCore.SignalR;

namespace lib.Coworkee.Application.Hubs
{
    public abstract class HubBase<T> : Hub<T> 
        where T : class
    {
        private const bool CheckGroups = false;

        private readonly ISessionProvider _sessionProvider;
        private readonly IUserService _userService;
        private readonly IRoleClaimService _roleService;
        private bool _groupsAdded;

        protected HubBase(ISessionProvider sessionProvider, IUserService userService, IRoleClaimService roleService)
        {
            _sessionProvider = sessionProvider;
            _userService = userService;
            _roleService = roleService;
        }

        public override async Task OnConnectedAsync()
        {
            await UpdateGroups(true, _sessionProvider.UserIdFromSession);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await UpdateGroups(false, _sessionProvider.UserIdFromSession);
            await base.OnDisconnectedAsync(exception);
        }

        // await HubConnection.InvokeAsync("JoinGroups", CurrentUserId);
        protected async Task JoinGroups(string userId)
        {
            await UpdateGroups(true, userId);
        }

        // await HubConnection.InvokeAsync("LeaveGroups", CurrentUserId);
        protected async Task LeaveGroups(string userId)
        {
            await UpdateGroups(false, userId);
        }

        protected async Task JoinGroup(string group)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, group);
        }

        protected async Task LeaveGroup(string group)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
        }

        protected async Task UpdateGroups(bool add, string userId)
        {
            if ((_groupsAdded == add && CheckGroups) || string.IsNullOrEmpty(userId)) return;

            Func<string, Task> fn = add
                ? JoinGroup
                : LeaveGroup;
            
            await fn(_sessionProvider.SessionId);
            await fn(userId);
            
            var userRoles = (await _userService.GetRolesAsync(userId)).Data.UserRoles
                .Where(r => r.Selected).ToArray();
            var permissions = userRoles.SelectMany(r => _roleService.GetAllByRoleIdAsync(r.Id).Result.Data)
                .Select(r => r.ClaimValue).Distinct();

            await Task.WhenAll(permissions.Select(p => fn(p))); // For Target Permissions
            await Task.WhenAll(userRoles.Select(r => fn(r.RoleName))); // For Target Role by name
            await Task.WhenAll(userRoles.Select(r => fn(r.Id))); // For Target RoleId
            _groupsAdded = add;
        }
    }
}
