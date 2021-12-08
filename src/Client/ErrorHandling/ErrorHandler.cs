using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CleanArchitectureBase.Shared.Wrapper;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using MudBlazor;
using Newtonsoft.Json;
using Nextended.Core;

namespace CleanArchitectureBase.Client.ErrorHandling
{
    public class ErrorHandler: IErrorHandler
    {
        private readonly ISnackbar _snackbar;
        private readonly IStringLocalizer<ErrorHandler> _localizer;
        private readonly IDialogService _dialogService;
        private readonly ILogger<ErrorHandler> _logger;

        public ErrorHandler(ISnackbar snackbar, IStringLocalizer<ErrorHandler> localizer, IDialogService dialogService, ILogger<ErrorHandler> logger)
        {
            _snackbar = snackbar;
            _localizer = localizer;
            _dialogService = dialogService;
            _logger = logger;
        }

        public void ShowError(ClientProblemDetails details)
        {
            if (!string.IsNullOrEmpty(details.Detail))
            {
                _snackbar.Add(details.Title, Severity.Error, config =>
                {
                    config.VisibleStateDuration = 10000;
                    config.HideTransitionDuration = 500;
                    config.ShowTransitionDuration = 500;
                    config.Action = _localizer["Show Details"];
                    config.ShowCloseIcon = true;
                    config.CloseAfterNavigation = true;
                    config.ActionColor = Color.Error;
                    config.Onclick = snackbar =>
                    {
                        var parameters = new DialogParameters
                        {
                            {nameof(Shared.Dialogs.MessageDialog.Message), $"{details.Detail}"},
                            {nameof(Shared.Dialogs.MessageDialog.Icon), Icons.Filled.Error}
                        };
                        var options = new DialogOptions { CloseButton = true, DisableBackdropClick = false };
                        var dialog = _dialogService.Show<Shared.Dialogs.MessageDialog>(details.Title, parameters, options);
                        return dialog.Result;
                    };
                });
            }
            else
                ShowError(details.Title);
        }

        public void ShowError(params string[] errors)
        {
            ShowErrors(errors);
        }

        public void ShowErrors(IEnumerable<string> errors)
        {
            foreach (var message in errors.Where(s => !string.IsNullOrWhiteSpace(s)))
                _snackbar.Add(_localizer[message], Severity.Error);
        }

        public async Task<bool> HandleAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content?.ReadAsStringAsync();
                var details = Check.TryCatch<ClientProblemDetails, Exception>(() => JsonConvert.DeserializeObject<ClientProblemDetails>(content));
                if (details?.Title != null)
                    ShowError(details);
                else
                    ShowError(content);
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
            _logger.LogError("Error:ProcessError - Type: {Type} Message: {Message}", exception.GetType(), exception.Message);
            return Task.CompletedTask;
        }
    }
}