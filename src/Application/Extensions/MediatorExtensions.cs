using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Hubs;
using CleanArchitectureBase.Application.Hubs.Events.Base;
using MediatR;

namespace CleanArchitectureBase.Application.Extensions
{
    public static class MediatorExtensions
    {
        public static Task PublishClientEvents(this IMediator mediator, CancellationToken cancellationToken = default, params ClientEventBase[] clientEvents)
        {
            return Task.WhenAll(clientEvents.Select(e => PublishClientEvent(mediator, e, cancellationToken)));
        }
        public static Task PublishClientEvents(this IMediator mediator, EventTarget targetOverwrite, CancellationToken cancellationToken = default, params ClientEventBase[] clientEvents)
        {
            return Task.WhenAll(clientEvents.Select(e => PublishClientEvent(mediator, e, targetOverwrite, cancellationToken)));
        }

        public static Task PublishClientEvents(this IMediator mediator, params ClientEventBase[] clientEvents)
        {
            return PublishClientEvents(mediator, default(CancellationToken), clientEvents);
        }

        public static Task PublishClientEvents(this IMediator mediator, EventTarget targetOverwrite, params ClientEventBase[] clientEvents)
        {
            return PublishClientEvents(mediator, targetOverwrite, default, clientEvents);
        }
        
        public static Task PublishClientEvents(this IMediator mediator, EventTarget targetOverwrite, IEnumerable<ClientEventBase> clientEvents, CancellationToken cancellationToken = default)
        {
            return PublishClientEvents(mediator, targetOverwrite, cancellationToken, clientEvents.ToArray());
        }

        public static Task PublishClientEvents(this IMediator mediator, IEnumerable<ClientEventBase> clientEvents, CancellationToken cancellationToken = default)
        {
            return PublishClientEvents(mediator, cancellationToken, clientEvents.ToArray());
        }

        public static Task PublishClientEvent<TEvent>(this IMediator mediator, TEvent clientEvent, CancellationToken cancellationToken = default) 
            where TEvent: ClientEventBase
        {
            return mediator.Publish(new ClientEventNotification(clientEvent), cancellationToken);
        }

        public static Task PublishClientEvent<TEvent>(this IMediator mediator, TEvent clientEvent, EventTarget targetOverwrite, CancellationToken cancellationToken = default)
            where TEvent : ClientEventBase
        {
            return mediator.Publish(new ClientEventNotification(clientEvent, targetOverwrite), cancellationToken);
        }
    }
}