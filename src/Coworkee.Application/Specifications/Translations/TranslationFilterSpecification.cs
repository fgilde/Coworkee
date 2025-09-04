using Coworkee.Application.Specifications.Base;
using Coworkee.Domain.Entities.Localization;

namespace Coworkee.Application.Specifications.Translations
{
    public class TranslationFilterSpecification : SpecificationBase<Translation>
    {
        public TranslationFilterSpecification(string searchString, string culture = null)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = p => p.Key != null && (p.Key.Contains(searchString) || p.Value.Contains(searchString)) && (culture == null || p.CultureCode == culture);
            }
            else
            {
                Criteria = p => p.Key != null && (culture == null || p.CultureCode == culture);
            }
        }
    }
}