using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Coworkee.Shared.Wrapper;

namespace Coworkee.Client.ErrorHandling
{
    public interface IErrorHandler
    {
        public void ShowError(params string[] errors);
        public void ShowErrors(IEnumerable<string> errors);
        public bool IsSuccessFull(IResult result, bool displayErrors = true);
        public Task<bool> HandleAsync(HttpResponseMessage response);
        public Task HandleAsync(Exception exception);
    }
}