using System;
using System.Net;

namespace CleanArchitectureBase.Application.Common.Exceptions
{
    public class ApiException : Exception
    {
        public ApiException(HttpStatusCode statusCode = HttpStatusCode.InternalServerError, params object[] arguments)
        {
            Arguments = arguments;
            StatusCode = (int)statusCode;
        }

        public ApiException(
            string messageOrResourceKey,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
            params object[] arguments)
            : base(messageOrResourceKey)
        {
            Arguments = arguments;
            StatusCode = (int)statusCode;
        }

        public ApiException(
            string messageOrResourceKey,
            Exception innerException,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
            params object[] arguments)
            : base(messageOrResourceKey, innerException)
        {
            Arguments = arguments;
            StatusCode = (int)statusCode;
        }

        public object[] Arguments { get; protected set; }

        public int? StatusCode { get; set; }
    }
}