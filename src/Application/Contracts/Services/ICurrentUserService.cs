using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Coworkee.Application.Common.Models.Identity;
using Coworkee.Application.Contracts.Common;

namespace Coworkee.Application.Contracts.Services;

public interface ICurrentUserService : IService
{
    string UserId { get; }
    string[] RoleIds { get; }
    List<KeyValuePair<string, string>> Claims { get; }
    ClaimsPrincipal Principal { get; }
    UserResponse CurrentUser();
    Task<IDisposable> AsSystemUser();
    Task<IDisposable> AsUser(string userId);
}