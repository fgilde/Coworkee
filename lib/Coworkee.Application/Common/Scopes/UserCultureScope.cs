using AKSoftware.Localization.MultiLanguages;
using lib.Coworkee.Application.Common.Models.Identity;

namespace lib.Coworkee.Application.Common.Scopes;

public class UserCultureScope : CultureScope
{
    public UserCultureScope(UserResponse user, ILanguageContainerService languageContainerService) : base(user?.UserInfo?.Language, languageContainerService)
    {}
}