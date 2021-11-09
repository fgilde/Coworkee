using System.Collections.Generic;
using System.Security.Claims;
using CleanArchitectureBase.Application.Interfaces.Common;
using CleanArchitectureBase.Application.Responses.Identity;

namespace CleanArchitectureBase.Application.Interfaces.Services
{
    public interface ICurrentUserService : IService
    {
        string UserId { get; }
        string[] RoleIds { get; }
        List<KeyValuePair<string, string>> Claims { get; }
        ClaimsPrincipal Principal { get; }
        UserResponse CurrentUser();
    }
}