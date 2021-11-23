using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Exceptions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitectureBase.Application
{
    public static partial class Errors
    {
        public static async Task<IdentityResult> EnsureSuccess(this Task<IdentityResult> identityResult)
        {
            return (await identityResult).EnsureSuccess();
        }

        public static IdentityResult EnsureSuccess(this IdentityResult identityResult)
        {
            if (!identityResult.Succeeded)
                throw IdentityFail(identityResult);
            return identityResult;
        }

        public static ValidationException ValidationFailed(
            params KeyValuePair<string, string>[] messages)
        {
            return ValidationFailed(messages.Select(s => new ValidationFailure(s.Key, s.Value)).ToArray());
        }
        public static ValidationException ValidationFailed(
            params (string property, string message)[] messages)
        {
            return ValidationFailed(messages.Select(s => new ValidationFailure(s.property, s.message)).ToArray());
        }
        public static ValidationException ValidationFailed(
            params string[] messages)
        {
            return ValidationFailed(messages.Select(s => new ValidationFailure(s, s)).ToArray());
        }
        public static ValidationException ValidationFailed(
            params ValidationFailure[] failures)
        {
            return new ValidationException(failures);
        }

        public static Exception Create(
            string resourceKey,
            Exception inner,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
            params object[] arguments)
        {
            return new ApiException(resourceKey, inner, statusCode, arguments);
        }

        public static Exception Create(
            string resourceKey,
            params object[] arguments)
        {
            return new ApiException(resourceKey, null, HttpStatusCode.InternalServerError, arguments);
        }

        
        public static Exception Create(
            string resourceKey,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
            params object[] arguments)
        {
            return Create(resourceKey, null, statusCode, arguments);
        }


        public static Exception NotFound(string message, params object[] arguments)
        {
            return new NotFoundException(message);
        }

        public static Exception IdentityFail(IdentityResult identityResult)
        {
            return ValidationFailed(identityResult.Errors.Select(e => (e.Code, e.Description)).ToArray());
        }
    }
}