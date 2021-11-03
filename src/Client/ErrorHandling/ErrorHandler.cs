using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CleanArchitectureBase.Client.Infrastructure.ErrorHandling;
using CleanArchitectureBase.Shared.Wrapper;
using MudBlazor;
using Newtonsoft.Json;
using Nextended.Core;

namespace CleanArchitectureBase.Client.ErrorHandling
{
    public class ErrorHandler: IErrorHandler
    {
        private readonly ISnackbar _snackbar;

        public ErrorHandler(ISnackbar snackbar)
        {
            _snackbar = snackbar;
        }

        public void ShowError(params string[] errors)
        {
            ShowErrors(errors);
        }

        public void ShowErrors(IEnumerable<string> errors)
        {
            foreach (var message in errors)
                _snackbar.Add(message, Severity.Error);
        }

        public async Task<bool> HandleAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content?.ReadAsStringAsync();
                var details = Check.TryCatch<ClientProblemDetails, Exception>(() => JsonConvert.DeserializeObject<ClientProblemDetails>(content));
                _snackbar.Add(details?.Title ?? content, Severity.Error);
                //if (details != null && response.StatusCode != HttpStatusCode.InternalServerError)
                //{
                //    // Error Handled // TODO: Find better solution
                //    response.StatusCode = HttpStatusCode.OK;
                //}
            }

            return response.IsSuccessStatusCode;
        }

        public bool IsSuccessFull(IResult result)
        {
            if (!result.Succeeded)
                ShowError(result.Messages.ToArray());
            return result.Succeeded;
        }

        public Task HandleAsync(Exception exception)
        {
            ShowError(exception.Message);
            return Task.CompletedTask;
        }
    }
}