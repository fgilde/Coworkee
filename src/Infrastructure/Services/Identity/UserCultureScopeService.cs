using AKSoftware.Localization.MultiLanguages;
using CleanArchitectureBase.Application.Common.Models.Identity;
using CleanArchitectureBase.Application.Common.Scopes;
using CleanArchitectureBase.Application.Contracts.Attributes;
using CleanArchitectureBase.Application.Contracts.Services.Identity;

namespace CleanArchitectureBase.Infrastructure.Services.Identity;

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