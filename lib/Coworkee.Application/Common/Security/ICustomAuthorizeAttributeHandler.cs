using System.Collections.Generic;
using System.Threading.Tasks;

namespace lib.Coworkee.Application.Common.Security
{
    public interface ICustomAuthorizeAttributeHandler
    {
        Task<bool> IsAuthorizedForAsync(ICustomAuthorizeAttribute attribute);
        Task EnsureIsAuthorizedForAsync(ICustomAuthorizeAttribute attribute);
        Task<bool> IsAuthorizedForAllAsync(IEnumerable<ICustomAuthorizeAttribute> attributes);
        Task EnsureIsAuthorizedForAllAsync(IEnumerable<ICustomAuthorizeAttribute> attributes);
    }
}