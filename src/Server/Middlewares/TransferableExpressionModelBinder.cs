using System;
using System.Threading.Tasks;
using CleanArchitectureBase.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace CleanArchitectureBase.Server.Middlewares;

public class TransferableExpressionModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var name = bindingContext.FieldName;
        var valueProviderResult = bindingContext.ValueProvider.GetValue(name);
        if (valueProviderResult.FirstValue == null && name != "$filter")
            valueProviderResult = bindingContext.ValueProvider.GetValue("$filter");
        var filter = valueProviderResult.FirstValue;
        
        if (!string.IsNullOrEmpty(filter))
        {
            var result = Activator.CreateInstance(bindingContext.ModelType, filter);
            bindingContext.Result = ModelBindingResult.Success(result);
        }

        return Task.CompletedTask;
    }
}

public class TransferableExpressionModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        return context?.Metadata?.ModelType?.IsGenericType == true && context.Metadata.ModelType.GetGenericTypeDefinition() == typeof(TransferableExpression<>)
            ? new BinderTypeModelBinder(typeof(TransferableExpressionModelBinder)) : null;
    }
}

public class FromOdataFilterAttribute : FromQueryAttribute
{
    public FromOdataFilterAttribute()
    {
        Name = "$filter";
    }
}