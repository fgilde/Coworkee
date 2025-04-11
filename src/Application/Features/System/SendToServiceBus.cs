using System;
using System.Threading;
using System.Threading.Tasks;
using Coworkee.Application.Contracts;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

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
            private readonly IServiceProvider _provider;

            public Handler(IServiceProvider provider)
            {
                _provider = provider;
            }

            public async Task Handle(Request request, CancellationToken cancellationToken)
            {
                var bus = _provider.GetRequiredService<IServiceBus>();
                await bus.SendMessageAsync(request.Queue, request.Content, cancellationToken);
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
