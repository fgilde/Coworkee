using System.Collections.Generic;
using System.Security.Claims;
using CleanArchitectureBase.Application.Contracts.Common;
using CleanArchitectureBase.Application.Responses.Identity;

namespace CleanArchitectureBase.Application.Contracts.Services
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