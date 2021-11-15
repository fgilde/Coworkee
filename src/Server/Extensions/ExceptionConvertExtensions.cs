using System;
using CleanArchitectureBase.Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureBase.Server.Extensions
{
    public static class ExceptionConvertExtensions
    {
        public static IActionResult ToActionResult(this Exception exception)
        {
            return exception switch
            {
                ApiException ex => ex.ToActionResult(),
                ValidationException ex => ex.ToActionResult(),
                NotFoundException ex => ex.ToActionResult(),
                UnauthorizedAccessException ex => ex.ToActionResult(),
                ForbiddenAccessException ex => ex.ToActionResult(),
                _ => CreateDefault(exception)
            };
        }

        private static IActionResult CreateDefault(Exception exception)
        {
            return new ObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An error occurred while processing your request.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        public static IActionResult ToActionResult(this ValidationException exception)
        {
            return new BadRequestObjectResult(new ValidationProblemDetails(exception.Errors)
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            });
        }

        public static IActionResult ToActionResult(this NotFoundException exception)
        {
            return new NotFoundObjectResult(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "The specified resource was not found.",
                Detail = exception.Message
            });
        }

        public static IActionResult ToActionResult(this UnauthorizedAccessException exception)
        {
            return new ObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
            })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
        }

        public static IActionResult ToActionResult(this ApiException exception)
        {
            return new ObjectResult(new ProblemDetails
            {
                Status = exception.StatusCode,
                Title = exception.Message,
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
            })
            {
                StatusCode = exception.StatusCode
            };
        }

        public static IActionResult ToActionResult(this ForbiddenAccessException exception)
        {
            return new ObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Title = "Forbidden",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
            })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }
    }
}