using System;
using System.Threading;
using System.Threading.Tasks;
using lib.Coworkee.Application.Contracts.Attributes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nextended.Core.Attributes;

namespace Coworkee.Application.Common.Behaviours
{
    [ProtectUnused(typeof(UnhandledExceptionBehaviour<,>))]
    [RegisterAs(typeof(IPipelineBehavior<,>), ServiceLifetime = ServiceLifetime.Transient)]
    public class UnhandledExceptionBehaviour<TRequest, TResponse>(ILogger<TRequest> logger)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
                return await next();
            }
            catch (Exception ex)
            {
                var requestName = typeof(TRequest).Name;

                logger.LogError(ex, "Coworkee Request: Unhandled Exception for Request {Name} {@Request}", requestName, request);

                throw;
            }
        }
    }
}