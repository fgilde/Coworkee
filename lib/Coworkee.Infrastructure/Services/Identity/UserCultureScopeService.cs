using AKSoftware.Localization.MultiLanguages;
using lib.Coworkee.Application.Common.Models.Identity;
using lib.Coworkee.Application.Common.Scopes;
using lib.Coworkee.Application.Contracts.Services.Identity;
using Nextended.Core.Attributes;

namespace lib.Coworkee.Infrastructure.Services.Identity;

[RegisterAs(typeof(IUserCultureScopeService))]
public class UserCultureScopeService : IUserCultureScopeService
{
    private readonly IUserService _userService;
    private readonly ILanguageContainerService _languageContainerService;

    public UserCultureScopeService(
        IUserService userService,
        ILanguageContainerService languageContainerService)
    {
        _userService = userService;
        _languageContainerService = languageContainerService;
    }

    public UserCultureScope CreateUserCultureScope(string userId) => CreateUserCultureScope(_userService.Get(userId));

    public UserCultureScope CreateUserCultureScope(UserResponse user) => new UserCultureScope(user, _languageContainerService);
}