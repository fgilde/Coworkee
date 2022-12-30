using Coworkee.Application.Common.Models.Identity;
using Coworkee.Application.Common.Scopes;

namespace Coworkee.Application.Contracts.Services.Identity;

public interface IUserCultureScopeService
{
    UserCultureScope CreateUserCultureScope(string userId);
    UserCultureScope CreateUserCultureScope(UserResponse user);
}