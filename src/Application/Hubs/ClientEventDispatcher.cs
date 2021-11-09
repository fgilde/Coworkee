using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Hubs.Events.Base;
using CleanArchitectureBase.Application.Interfaces;
using CleanArchitectureBase.Shared.Constants.Application;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace CleanArchitectureBase.Application.Hubs
{
    public class ClientEventDispatcher : INotificationHandler<ClientEventNotification>
    {
        private readonly IHubContext<ClientEventHub> _hubContext;
        private readonly ISessionProvider _sessionProvider;

        public ClientEventDispatcher(IHubContext<ClientEventHub> hubContext, ISessionProvider sessionProvider)
        {
            _hubContext = hubContext;
            _sessionProvider = sessionProvider;
        }

        public Task Handle(ClientEventNotification clientEventNotification, CancellationToken cancellationToken)
        {
            var targetProxy = GetTargetProxy(clientEventNotification.Target);

            return Task.WhenAll(
                targetProxy.SendAsync(ApplicationConstants.SignalR.ClientEventName, clientEventNotification, cancellationToken), 
                targetProxy.SendAsync(clientEventNotification.EventName, clientEventNotification.Arguments, cancellationToken)
            );
        }

        private IClientProxy GetTargetProxy(EventTarget target)
        {
            return target switch
            {
                EventTarget.Current => _hubContext.Clients.Group(_sessionProvider.SessionId),
                EventTarget.All => _hubContext.Clients.All,
                _ => _hubContext.Clients.Group(_sessionProvider.SessionId),
            };
        }
    }
}
