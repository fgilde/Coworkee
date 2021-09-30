using System;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Security;
using CleanArchitectureBase.Server.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CleanArchitectureBase.Server.Filters
{
    public class CustomAuthorizeAttributeFilter: IAsyncAuthorizationFilter
    {
        private readonly ICustomAuthorizeAttribute _attribute;
        private readonly ICustomAuthorizeAttributeHandler _handler;

        public CustomAuthorizeAttributeFilter(ICustomAuthorizeAttribute attribute, 
            ICustomAuthorizeAttributeHandler handler)
        {
            _attribute = attribute;
            _handler = handler;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            try
            {
                await _handler.EnsureIsAuthorizedForAsync(_attribute);
            }
            catch (Exception e)
            {
                context.Result = e.ToActionResult();
            }
        }
    }
}