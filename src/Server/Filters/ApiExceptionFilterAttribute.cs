using System;
using System.Collections.Generic;
using System.Linq;
using lib.Coworkee.Application.Common.Exceptions;
using Coworkee.Server.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Coworkee.Server.Filters
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger _logger;
        private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;

        public ApiExceptionFilterAttribute(ILogger<ApiExceptionFilterAttribute> logger)
        {
            _logger = logger;
            _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
            {
                { typeof(ApiException), HandleKnownException },
                { typeof(ValidationException), HandleKnownException },
                { typeof(NotFoundException), HandleKnownException },
                { typeof(UnauthorizedAccessException), HandleKnownException },
                { typeof(ForbiddenAccessException), HandleKnownException },
            };
        }

        public override void OnException(ExceptionContext context)
        {
            HandleException(context);
            LogInformations(context);
            base.OnException(context);
        }

        private void HandleException(ExceptionContext context)
        {
            var handler = FindHandler(context);
            if (handler != null)
                handler.Invoke(context);
            else if (!context.ModelState.IsValid)
                HandleInvalidModelStateException(context);
            else
                HandleUnknownException(context);
        }

        private void LogInformations(ExceptionContext context)
        {
            if (context?.Exception != null)
            {
                if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                {
                    // Log Controller Infos like [Post] Request to Method 'CreateDefinition' on 'PropertyDefinitionController' caused error
                    var controllerTypeInfo = controllerActionDescriptor.ControllerTypeInfo;
                    _logger.LogError(context.Exception, $"Error in  {controllerTypeInfo}: [{context.HttpContext.Request.Method}] Request to Method '{controllerActionDescriptor.ActionName}' on '{controllerActionDescriptor.ControllerName}Controller' caused error '{context.Exception.Message}'");
                }
                var type = context.Exception.TargetSite?.DeclaringType;
                var declaringType = type?.DeclaringType ?? type ?? GetType();
                _logger.LogError(context.Exception, $"Error in {declaringType}: The {context.Exception.TargetSite?.MemberType} '{type?.Name}' in '{type?.DeclaringType?.Name}' caused the error '{context.Exception.Message}'");
            }
        }

        private Action<ExceptionContext> FindHandler(ExceptionContext context)
        {
            Type type = context.Exception.GetType();
            return _exceptionHandlers.Where(p => p.Key == type || p.Key.IsAssignableFrom(type)).Select(p => p.Value).FirstOrDefault();
        }

        private void HandleInvalidModelStateException(ExceptionContext context)
        {
            var details = new ValidationProblemDetails(context.ModelState)
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            };

            context.Result = new BadRequestObjectResult(details);

            context.ExceptionHandled = true;
        }


        private void HandleUnknownException(ExceptionContext context)
        {
            context.Result = context.Exception.ToActionResult();
            context.ExceptionHandled = false;
        }

        private void HandleKnownException(ExceptionContext context)
        {
            context.Result = context.Exception.ToActionResult();
            context.ExceptionHandled = true;
        }
    }
}
