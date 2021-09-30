using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Security;
using MediatR;

namespace CleanArchitectureBase.Application.Behaviours
{
    public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ICustomAuthorizeAttributeHandler _authorizeAttributeHandler;

        public AuthorizationBehaviour(ICustomAuthorizeAttributeHandler authorizeAttributeHandler)
        {
            _authorizeAttributeHandler = authorizeAttributeHandler;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            await _authorizeAttributeHandler.EnsureIsAuthorizedForAllAsync(request.GetType().GetCustomAttributes<CustomAuthorizeAttribute>());
            return await next();
        }
    }
}
