using System.Threading;
using System.Threading.Tasks;
using Coworkee.Application.Contracts;
using FluentValidation;
using MediatR;

namespace Coworkee.Application.Features.System
{
    public class SendToServiceBus
    {
        public class Request : IRequest
        {
            public string Queue { get; set; }
            public object Content { get; set; }
        }

        internal class Handler : IRequestHandler<Request>
        {
            private readonly IServiceBus bus;

            public Handler(IServiceBus bus)
            {
                this.bus = bus;
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                await bus.SendMessageAsync(request.Queue, request.Content, cancellationToken);
                return Unit.Value;
            }
        }

        internal class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Queue).NotEmpty();
            }
        }
    }
}
