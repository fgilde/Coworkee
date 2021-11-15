using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Extensions;
using CleanArchitectureBase.Application.Hubs.Events;
using MediatR;

namespace CleanArchitectureBase.Application.Common.Behaviours
{
    internal class ClientEventBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IMediator mediator;

        public ClientEventBehaviour(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            await mediator.PublishClientEvent(new BeforeRequest<TRequest>(request), cancellationToken);
            var response = await next();
            await mediator.PublishClientEvent(new AfterRequest<TRequest, TResponse>(request, response), cancellationToken);
            return response;
        }
    }
}
