using System.Threading;
using System.Threading.Tasks;
using lib.Coworkee.Application.Common.Extensions;
using lib.Coworkee.Application.Hubs.Events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nextended.Core.Attributes;

namespace lib.Coworkee.Application.Common.Behaviours
{
    [RegisterAs(typeof(IPipelineBehavior<,>), ServiceLifetime = ServiceLifetime.Transient)]
    internal class ClientEventBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IMediator mediator;

        public ClientEventBehaviour(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            await mediator.PublishClientEvent(new BeforeRequest<TRequest>(request), cancellationToken);
            var response = await next();
            await mediator.PublishClientEvent(new AfterRequest<TRequest, TResponse>(request, response), cancellationToken);
            return response;
        }
    }
}