using AKSoftware.Localization.MultiLanguages;
using CleanArchitectureBase.Application.Common.Models.Identity;

namespace CleanArchitectureBase.Application.Common.Scopes;

public class UserCultureScope : CultureScope
{
    public UserCultureScope(UserResponse user, ILanguageContainerService languageContainerService) : base(user?.UserInfo?.Language, languageContainerService)
    {}
}