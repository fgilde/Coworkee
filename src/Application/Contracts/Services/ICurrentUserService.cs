using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models.Identity;
using CleanArchitectureBase.Application.Contracts.Common;

namespace CleanArchitectureBase.Application.Contracts.Services
{
    public interface ICurrentUserService : IService
    {
        string UserId { get; }
        string[] RoleIds { get; }
        List<KeyValuePair<string, string>> Claims { get; }
        ClaimsPrincipal Principal { get; }
        UserResponse CurrentUser();
        Task<IDisposable> AsSystemUser();
    }
}