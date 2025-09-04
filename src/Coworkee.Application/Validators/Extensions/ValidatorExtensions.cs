using FluentValidation;
using FluentValidation.Validators;
using Microsoft.Extensions.Localization;

namespace Coworkee.Application.Validators.Extensions
{
    public static class ValidatorExtensions
    {
        public static IRuleBuilderOptions<T, string> MustBeJson<T>(this IRuleBuilder<T, string> ruleBuilder, IPropertyValidator<T, string> validator) where T : class
        {
            return ruleBuilder.SetValidator(validator);
        }

        public static IRuleBuilderOptions<TModel, TProperty> WithNotEmptyMessage<TModel, TProperty>(this IRuleBuilderOptions<TModel, TProperty> rule, IStringLocalizer l) where TModel : class
        {
            string message = "";
            return rule.When((instance, context) =>
            {
                var propertyName = context.PropertyName;
                LocalizedString localizedLabel = l[$"Label_{propertyName}"];
                message = string.Format(l[$"Field is required!"], localizedLabel.ResourceNotFound ? l[propertyName] : localizedLabel);
                return true;
            }).WithMessage((model, value) => message);

            // return rule.WithMessage((arg1, property) => "This field is required! " + property + " || " + arg1);
        }
    }
}