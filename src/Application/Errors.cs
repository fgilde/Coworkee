using System;
using System.Collections.Generic;
using System.Net;
using CleanArchitectureBase.Application.Common.Exceptions;
using FluentValidation.Results;

namespace CleanArchitectureBase.Application
{
    public partial class Errors
    {
        //public static ValidationException ValidationFailed(
        //    string resourceKey,
        //    params string[] arguments)
        //{
        //    var failures = new List<ValidationFailure>();
        //    failures.Add(new ValidationFailure("Items[0].Barcode", "Is duplicate"));
        //    return new ValidationException(failures);
        //}

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

    }
}