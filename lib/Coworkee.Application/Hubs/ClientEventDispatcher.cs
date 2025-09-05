using System.Threading;
using System.Threading.Tasks;
using lib.Coworkee.Application.Contracts;
using lib.Coworkee.Application.Hubs.Events.Base;
using lib.Coworkee.Shared.Constants.Application;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace lib.Coworkee.Application.Hubs
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
            if (target == EventTarget.All) return _hubContext.Clients.All;
            if (target == EventTarget.Current) return _hubContext.Clients.Group(_sessionProvider.SessionId);
            return _hubContext.Clients.Groups(target.Groups);
        }
    }
}
