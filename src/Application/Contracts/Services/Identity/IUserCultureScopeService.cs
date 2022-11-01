using CleanArchitectureBase.Application.Common.Models.Identity;
using CleanArchitectureBase.Application.Common.Scopes;

namespace CleanArchitectureBase.Application.Contracts.Services.Identity;

public interface IUserCultureScopeService
{
    UserCultureScope CreateUserCultureScope(string userId);
    UserCultureScope CreateUserCultureScope(UserResponse user);
}