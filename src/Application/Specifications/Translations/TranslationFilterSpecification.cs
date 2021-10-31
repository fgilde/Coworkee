using CleanArchitectureBase.Application.Specifications.Base;
using CleanArchitectureBase.Domain.Entities.Localization;

namespace CleanArchitectureBase.Application.Specifications.Translations
{
    public class TranslationFilterSpecification : HeroSpecification<Translation>
    {
        public TranslationFilterSpecification(string searchString)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = p => p.Key != null && (p.Key.Contains(searchString) || p.Value.Contains(searchString));
            }
            else
            {
                Criteria = p => p.Key != null;
            }
        }
    }
}