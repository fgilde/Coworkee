using System;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Hubs.Events.Base;
using Microsoft.AspNetCore.SignalR.Client;
using CleanArchitectureBase.Shared.Constants.Application;

namespace CleanArchitectureBase.Client.Extensions
{
    public static class HubExtensions
    {
        public static string On<TClientEvent>(this HubConnection hubConnection, Action<TClientEvent> handler)
            where TClientEvent : ClientEventBase, new()
        {
            var e = new TClientEvent();
            hubConnection.On(e.EventName, handler);
            return e.EventName;
        }

        public static string On<TClientEvent>(this HubConnection hubConnection, Func<TClientEvent, Task> handler)
            where TClientEvent : ClientEventBase, new()
        {
            var e = new TClientEvent();
            hubConnection.On(e.EventName, handler);
            return e.EventName;
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

        public static ValueTask TryDisposeAsync(this HubConnection hubConnection)
        {
            return ValueTask.CompletedTask;
            // return hubConnection?.DisposeAsync() ?? ValueTask.CompletedTask;
        }

        public static HubConnection BuildHubConnection(string backendOrigin)
        {
            return new HubConnectionBuilder()
                .WithUrl($"{backendOrigin}{ApplicationConstants.SignalR.EventHubUrl}")
                .WithAutomaticReconnect()
                .Build();
        }

        private static HubConnection TryInitialize(this HubConnection hubConnection, string backendOrigin)
        {
            return hubConnection ?? BuildHubConnection(backendOrigin);
        }
    }
}