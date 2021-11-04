using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using CleanArchitectureBase.Shared.Constants.Application;

namespace CleanArchitectureBase.Client.Extensions
{
    public static class HubExtensions
    {
        public static HubConnection TryInitialize(this HubConnection hubConnection, NavigationManager navigationManager)
        {
            if (hubConnection == null)
            {
                hubConnection = new HubConnectionBuilder()
                                  .WithUrl(navigationManager.ToAbsoluteUri(ApplicationConstants.SignalR.EventHubUrl))
                                  .Build();
            }
            return hubConnection;
        }

        public static async Task<HubConnection> EnsureStartedAsync(this HubConnection hubConnection, NavigationManager navigationManager)
        {
            hubConnection = hubConnection.TryInitialize(navigationManager);
            if (hubConnection.State == HubConnectionState.Disconnected)
            {
                await hubConnection.StartAsync();
            }

            return hubConnection;
        }
    }
}