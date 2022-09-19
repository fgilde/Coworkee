using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CleanArchitectureBase.Client.Extensions;
using CleanArchitectureBase.Client.Shared.Dialogs;
using CleanArchitectureBase.Client.Utils;
using CleanArchitectureBase.Shared.Wrapper;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using MudBlazor;
using Newtonsoft.Json;
using Nextended.Core;

namespace CleanArchitectureBase.Client.ErrorHandling
{
    public class ErrorHandler : IErrorHandler
    {
        private readonly ISnackbar _snackbar;
        private readonly IStringLocalizer<ErrorHandler> _localizer;
        private readonly IDialogService _dialogService;
        private readonly ILogger<ErrorHandler> _logger;
        private readonly NavigationManager _navigationManager;

        public ErrorHandler(ISnackbar snackbar,
            IStringLocalizer<ErrorHandler> localizer,
            IDialogService dialogService, ILogger<ErrorHandler> logger,
            NavigationManager navigationManager)
        {
            _snackbar = snackbar;
            _localizer = localizer;
            _dialogService = dialogService;
            _logger = logger;
            _navigationManager = navigationManager;
        }

        public void ShowError(ClientProblemDetails details)
        {
            if (!string.IsNullOrEmpty(details.Detail))
            {
                var actions = new[]
                {
                    new MessageDialog.DialogResultAction
                    {
                        Label = "Ask Google",
                        Variant = Variant.Filled,
                        Color = Color.Secondary,
                        Result =  DialogResult.Ok(true)
                    },
                    new MessageDialog.DialogResultAction
                    {
                        Label = "Close",
                        Color = Color.Error,
                        Variant = Variant.Filled,
                        Result = DialogResult.Cancel()
                    },
                };
                if (!Debug.IsDebug())
                {
                    actions = new[] { actions[1] }; // Close only if not in debug
                }
                _snackbar.Add(details.Title, Severity.Error, config =>
                {
                    config.VisibleStateDuration = 10000;
                    config.HideTransitionDuration = 500;
                    config.ShowTransitionDuration = 500;
                    config.Action = _localizer["Show Details"];
                    config.ShowCloseIcon = true;
                    config.CloseAfterNavigation = true;
                    config.ActionColor = Color.Error;
                    config.Onclick = async snackbar =>
                    {
                        var parameters = new DialogParameters
                        {
                            {nameof(Shared.Dialogs.MessageDialog.Message), $"{details.Detail}"},
                            {nameof(Shared.Dialogs.MessageDialog.Icon), Icons.Filled.Error},
                            {nameof(Shared.Dialogs.MessageDialog.Buttons), actions},
                        };
                        var options = new DialogOptions { CloseButton = true, DisableBackdropClick = false };
                        var dialog = _dialogService.Show<Shared.Dialogs.MessageDialog>(details.Title, parameters, options);
                        if (!(await dialog.Result).Cancelled)
                        {
                            _navigationManager.NavigateToUnknown($"https://www.google.de/search?q={Uri.EscapeDataString(details.Detail)}");
                        }

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

        public bool IsSuccessFull(IResult result, bool displayErrors = true)
        {
            if (!result.Succeeded && displayErrors)
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