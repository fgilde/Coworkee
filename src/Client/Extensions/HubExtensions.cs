using System;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Hubs.Events.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using CleanArchitectureBase.Shared.Constants.Application;

namespace CleanArchitectureBase.Client.Extensions
{
    public static class HubExtensions
    {
        public static IDisposable On<TClientEvent>(this HubConnection hubConnection, Action<TClientEvent> handler)
            where TClientEvent : ClientEventBase, new()
        {
            var e = new TClientEvent();
            return hubConnection.On(e.EventName, handler);
        }

        public static IDisposable On<TClientEvent>(this HubConnection hubConnection, Func<TClientEvent, Task> handler)
            where TClientEvent: ClientEventBase, new()
        {
            var e = new TClientEvent();
            return hubConnection.On(e.EventName, handler);
        }

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