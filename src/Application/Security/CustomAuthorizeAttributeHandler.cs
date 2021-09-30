using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Interfaces.Services;

namespace CleanArchitectureBase.Application.Security
{
    public class CustomAuthorizeAttributeHandler: ICustomAuthorizeAttributeHandler
    {
        private readonly IPermissionService _permissionService;
        
        public CustomAuthorizeAttributeHandler(
            IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        public async Task<bool> IsAuthorizedForAsync(ICustomAuthorizeAttribute attribute)
        {
            if (attribute != null)
            {
                return await _permissionService.HasPoliciesAsync(attribute.Policies, attribute.PolicyMatch)
                    && await _permissionService.HasRolesAsync(attribute.Roles, attribute.RoleMatch);
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
            await _permissionService.EnsureRolesAsync(attribute?.Roles, attribute?.RoleMatch ?? RoleMatch.Any);
            await _permissionService.EnsurePoliciesAsync(attribute?.Policies, attribute?.PolicyMatch ?? PolicyMatch.Any);
        }

        public async Task EnsureIsAuthorizedForAllAsync(IEnumerable<ICustomAuthorizeAttribute> attributes)
        {
            foreach (var attribute in attributes)
                await EnsureIsAuthorizedForAsync(attribute);
        }
    }
}