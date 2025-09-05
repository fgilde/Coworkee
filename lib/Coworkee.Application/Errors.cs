using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using lib.Coworkee.Application.Common.Exceptions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace lib.Coworkee.Application
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
            string messageOrKey,
            Exception inner,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
            params object[] arguments)
        {
            return new ApiException(TryFormat(messageOrKey, arguments), inner, statusCode, arguments);
        }

        public static Exception Create(
            string messageOrKey,
            params object[] arguments)
        {
            return new ApiException(TryFormat(messageOrKey, arguments), null, HttpStatusCode.InternalServerError, arguments);
        }

        
        public static Exception Create(
            string messageOrKey,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
            params object[] arguments)
        {
            return Create(messageOrKey, null, statusCode, arguments);
        }

        public static Exception Forbidden() => new ForbiddenAccessException();
        public static Exception Unauthorized(string message = null) => new UnauthorizedAccessException(message);
        public static Exception NotFound(string message, params object[] arguments) => new NotFoundException(message);
        public static Exception IdentityFail(IdentityResult identityResult) => ValidationFailed(identityResult.Errors.Select(e => (e.Code, e.Description)).ToArray());
        private static string TryFormat(string s, object[] arguments) => arguments?.Any() == true ? string.Format(s, arguments) : s;
    }
}