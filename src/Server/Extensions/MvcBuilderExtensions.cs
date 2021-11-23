using System.Linq;
using CleanArchitectureBase.Application.Configurations;
using CleanArchitectureBase.Application.Features.ExtendedAttributes.Commands.AddEdit;
using CleanArchitectureBase.Application.Validators.Features.ExtendedAttributes.Commands.AddEdit;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureBase.Server.Extensions
{
    internal static class MvcBuilderExtensions
    {
        internal static IMvcBuilder AddValidators(this IMvcBuilder builder)
        {
            builder.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<ServerConfiguration>());
            return builder;
        }

        internal static void AddExtendedAttributesValidators(this IServiceCollection services)
        {
            #region AddEditExtendedAttributeCommandValidator

            var addEditExtendedAttributeCommandValidatorType = typeof(AddEditExtendedAttributeCommandValidator<,,,>);
            var validatorTypes = addEditExtendedAttributeCommandValidatorType
                .Assembly
                .GetExportedTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.BaseType?.IsGenericType == true)
                .Select(t => new
                {
                    BaseGenericType = t.BaseType,
                    CurrentType = t
                })
                .Where(t => t.BaseGenericType?.GetGenericTypeDefinition() == typeof(AddEditExtendedAttributeCommandValidator<,,,>))
                .ToList();

            foreach (var validatorType in validatorTypes)
            {
                var addEditExtendedAttributeCommandType = typeof(AddEditExtendedAttributeCommand<,,,>).MakeGenericType(validatorType.BaseGenericType.GetGenericArguments());
                var iValidator = typeof(IValidator<>).MakeGenericType(addEditExtendedAttributeCommandType);
                services.AddScoped(iValidator, validatorType.CurrentType);
            }

            #endregion AddEditExtendedAttributeCommandValidator
        }
    }
}