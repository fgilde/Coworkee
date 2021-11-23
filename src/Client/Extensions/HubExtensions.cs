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
        public static HubConnection On<TClientEvent>(this HubConnection hubConnection, Action<TClientEvent> handler)
            where TClientEvent : ClientEventBase, new()
        {
            var e = new TClientEvent();
            hubConnection.On(e.EventName, handler);
            return hubConnection;
        }

        public static HubConnection On<TClientEvent>(this HubConnection hubConnection, Func<TClientEvent, Task> handler)
            where TClientEvent: ClientEventBase, new()
        {
            var e = new TClientEvent();
            hubConnection.On(e.EventName, handler);
            return hubConnection;
        }

        public static HubConnection TryInitialize(this HubConnection hubConnection, string backendOrigin)
        {
            if (hubConnection == null)
            {
                hubConnection = new HubConnectionBuilder()
                                  .WithUrl($"{backendOrigin}{ApplicationConstants.SignalR.EventHubUrl}")
                                  .Build();
            }
            return hubConnection;
        }

        public static async Task<HubConnection> EnsureStartedAsync(this HubConnection hubConnection, string backendOrigin)
        {
            hubConnection = hubConnection.TryInitialize(backendOrigin);
            if (hubConnection.State == HubConnectionState.Disconnected)
            {
                await hubConnection.StartAsync();
            }

            return hubConnection;
        }
    }
}