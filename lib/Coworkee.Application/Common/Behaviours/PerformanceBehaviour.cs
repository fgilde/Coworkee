using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Coworkee.Application.Contracts.Attributes;
using Coworkee.Application.Contracts.Services;
using Coworkee.Application.Contracts.Services.Account;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nextended.Core.Attributes;

namespace Coworkee.Application.Common.Behaviours
{
    [ProtectUnused(typeof(PerformanceBehaviour<,>))]
    [RegisterAs(typeof(IPipelineBehavior<,>), ServiceLifetime = ServiceLifetime.Transient)]
    public class PerformanceBehaviour<TRequest, TResponse>(
        ILogger<TRequest> logger,
        ICurrentUserService currentUserService,
        IAccountService identityService)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly Stopwatch _timer = new();

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _timer.Start();

            var response = await next();

            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            if (elapsedMilliseconds > 500)
            {
                var requestName = typeof(TRequest).Name;
                var userId = currentUserService.UserId ?? string.Empty;
                var userName = string.Empty;

                if (!string.IsNullOrEmpty(userId))
                {
                    userName = await identityService.GetUserNameAsync(userId);
                }

                logger.LogWarning("Coworkee Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@Request}",
                    requestName, elapsedMilliseconds, userId, userName, request);
            }

            return response;
        }
    }
}
