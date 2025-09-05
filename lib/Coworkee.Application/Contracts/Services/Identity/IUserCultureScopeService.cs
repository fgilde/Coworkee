using lib.Coworkee.Application.Common.Models.Identity;
using lib.Coworkee.Application.Common.Scopes;

namespace lib.Coworkee.Application.Contracts.Services.Identity;

public interface IUserCultureScopeService
{
    UserCultureScope CreateUserCultureScope(string userId);
    UserCultureScope CreateUserCultureScope(UserResponse user);
}