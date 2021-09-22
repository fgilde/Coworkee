using System;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace CleanArchitectureBase.Application.Hubs
{
    public abstract class HubBase : Hub
    {
        private readonly ISessionProvider _sessionProvider;

        protected HubBase(ISessionProvider sessionProvider)
        {
            _sessionProvider = sessionProvider;
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, _sessionProvider.SessionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, _sessionProvider.SessionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
