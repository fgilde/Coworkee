using AKSoftware.Localization.MultiLanguages;
using Coworkee.Application.Common.Models.Identity;

namespace Coworkee.Application.Common.Scopes;

public class UserCultureScope : CultureScope
{
    public UserCultureScope(UserResponse user, ILanguageContainerService languageContainerService) : base(user?.UserInfo?.Language, languageContainerService)
    {}
}