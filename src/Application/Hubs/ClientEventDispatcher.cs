using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Interfaces;
using CleanArchitectureBase.Shared.Constants.Application;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace CleanArchitectureBase.Application.Hubs
{
    public class ClientEventDispatcher : INotificationHandler<ClientEvent>
    {
        private readonly IHubContext<ClientEventHub> _hubContext;
        private readonly ISessionProvider _sessionProvider;

        public ClientEventDispatcher(IHubContext<ClientEventHub> hubContext, ISessionProvider sessionProvider)
        {
            _hubContext = hubContext;
            _sessionProvider = sessionProvider;
        }

        public Task Handle(ClientEvent clientEvent, CancellationToken cancellationToken)
        {
            var targetProxy = clientEvent.Target == TargetClient.Current
                ? _hubContext.Clients.Group(_sessionProvider.SessionId)
                : _hubContext.Clients.All;
            return targetProxy.SendAsync(ApplicationConstants.EventNames.ClientEventName, clientEvent, cancellationToken);
        }
    }
}
