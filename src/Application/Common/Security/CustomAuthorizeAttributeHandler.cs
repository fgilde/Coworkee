using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lib.Coworkee.Application.Contracts.Attributes;
using lib.Coworkee.Application.Contracts.Services;
using Microsoft.Extensions.DependencyInjection;
using Nextended.Core.Attributes;

namespace Coworkee.Application.Common.Security
{
    [ProtectUnused(typeof(CustomAuthorizeAttributeHandler))]
    [RegisterAs(typeof(ICustomAuthorizeAttributeHandler), ServiceLifetime = ServiceLifetime.Transient)]
    public class CustomAuthorizeAttributeHandler(IPermissionService permissionService)
        : ICustomAuthorizeAttributeHandler
    {
        public async Task<bool> IsAuthorizedForAsync(ICustomAuthorizeAttribute attribute)
        {
            if (attribute != null)
            {
                return await permissionService.HasPoliciesAsync(attribute.Policies, attribute.PolicyMatch)
                    && await permissionService.HasRolesAsync(attribute.Roles, attribute.RoleMatch);
            }
            return true;
        }

        public async Task<bool> IsAuthorizedForAllAsync(IEnumerable<ICustomAuthorizeAttribute> attributes)
        {
            var customAuthorizeAttributes = attributes?.ToList() ?? new List<ICustomAuthorizeAttribute>();
            if (!customAuthorizeAttributes.Any())
                return true;

            foreach (var attribute in customAuthorizeAttributes)
            {
                if (!await IsAuthorizedForAsync(attribute))
                    return false;
            }

            return true;
        }

        public async Task EnsureIsAuthorizedForAsync(ICustomAuthorizeAttribute attribute)
        {            
            await permissionService.EnsureRolesAsync(attribute?.Roles, attribute?.RoleMatch ?? RoleMatch.Any);
            await permissionService.EnsurePoliciesAsync(attribute?.Policies, attribute?.PolicyMatch ?? PolicyMatch.Any);
        }

        public async Task EnsureIsAuthorizedForAllAsync(IEnumerable<ICustomAuthorizeAttribute> attributes)
        {
            foreach (var attribute in attributes)
                await EnsureIsAuthorizedForAsync(attribute);
        }
    }
}