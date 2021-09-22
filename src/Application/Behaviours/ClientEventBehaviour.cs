using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace CleanArchitectureBase.Application.Behaviours
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
            var requestInfo = new
            {
                Name = typeof(TRequest).FullName,
                Request = request
            };
            await mediator.Publish(new ClientEvent(TargetClient.Current, "BeforeRequest", requestInfo), cancellationToken);
            var response = await next();
            await mediator.Publish(new ClientEvent(TargetClient.Current, "OnResponse", new
            {
                Request = requestInfo,
                Response = response
            }), cancellationToken);
            return response;
        }
    }
}
