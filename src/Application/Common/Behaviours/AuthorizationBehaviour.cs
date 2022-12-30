using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Coworkee.Application.Common.Security;
using MediatR;

namespace Coworkee.Application.Common.Behaviours
{
    public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ICustomAuthorizeAttributeHandler _authorizeAttributeHandler;

        public AuthorizationBehaviour(ICustomAuthorizeAttributeHandler authorizeAttributeHandler)
        {
            _authorizeAttributeHandler = authorizeAttributeHandler;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            await _authorizeAttributeHandler.EnsureIsAuthorizedForAllAsync(request.GetType().GetCustomAttributes<CustomAuthorizeAttribute>());
            return await next();
        }
    }
}