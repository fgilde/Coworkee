using System;
using System.Linq;
using Coworkee.Application.Requests.Identity;
using MudBlazor;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using Coworkee.Application.Configurations;
using Microsoft.AspNetCore.Components;
using Coworkee.Client.Extensions;
using Coworkee.Shared.Constants.Application;

namespace Coworkee.Client.Pages.Authentication
{
    public partial class Login
    {
        [Parameter] public string Message { get; set; }
        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private TokenRequest _tokenModel = new();
        private bool _processing;
        protected override async Task OnInitializedAsync()
        {
            _tokenModel.Email = _navigationManager.ReadQueryParam("email");
            SetMessage();

            if(ShouldRedirectToKeycloak())            
                RedirectToKeycloak();
            
            await base.OnInitializedAsync();
        }

        private bool ShouldRedirectToKeycloak()
        {
            if(_navigationManager.Uri?.Contains("#!internal") == true)            
                return false;
            
            return (_config?.ServerConfiguration?.LoginSettings?.LoginMode == LoginMode.External || _navigationManager.Uri?.Contains("#!external") == true) && _config?.ServerConfiguration?.KeycloakEnabled == true;
        }

        private void RedirectToKeycloak()
        {
            _navigationManager.NavigateTo(KeyCloakHref(), true);
        }

        private async Task SubmitAsync()
        {
            try
            {
                _processing = true;
                var result = await _clientAuthenticationManager.Login(_tokenModel);
                if (_errorService.IsSuccessFull(result, false))
                {
                    _snackBar.Add(string.Format(_localizer["Welcome {0}"], _tokenModel.Email), Severity.Success);
                    _navigationManager.NavigateToReturnUrlIf("/");
                }
                else
                {
                    var message = string.Join($"{Environment.NewLine}", result.Messages.Select(m => _localizer[m]));
                    SetMessage(string.Format(message, $"/account/forgot-password?email={_tokenModel.Email}&email-confirm=true"), Severity.Error);
                }
            }
            finally
            {
                _processing = false;
            }
        }

        private string _message;
        private Severity _messageSeverity = Severity.Info;
        private bool _passwordVisibility;
        private InputType _passwordInput = InputType.Password;
        private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

        void SetMessage()
        {
            _message = Message;
            _messageSeverity = Severity.Info;
            var confirmationResult = _navigationManager.ReadQueryParam("email-confirmation-result");
            if (confirmationResult != null)
            {
                var email = _config.ServerConfiguration.ContactAddress;
                var success = confirmationResult == "true";
                var message = (success ? _localizer["Your e-mail address has been confirmed!"] : _localizer["Your email address could not be verified. If you have any problems, please contact {0}", email]).ToString();
                var active = _navigationManager.ReadQueryParam("activated");
                if (success && !string.IsNullOrWhiteSpace(active))
                {
                    message += " " + (active == "true" ? _localizer["You can now Login with your credentials"] : _localizer["As soon as your account has been activated you can log in"]);
                }
                SetMessage(message, success);
            }
            else
            {
                var s = _snackBar.ShownSnackbars.FirstOrDefault();
                if (s != null)
                {
                    _message = s.Message;
                    _messageSeverity = s.Severity;
                }
            }
        }

        void SetMessage(string message, bool? success)
        {
            SetMessage(message, success.HasValue ? (success.Value ? Severity.Success : Severity.Error) : Severity.Info);
        }

        void SetMessage(string message, Severity severity)
        {
            _message = message;
            _messageSeverity = severity;
        }

        void TogglePasswordVisibility()
        {
            if (_passwordVisibility)
            {
                _passwordVisibility = false;
                _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
                _passwordInput = InputType.Password;
            }
            else
            {
                _passwordVisibility = true;
                _passwordInputIcon = Icons.Material.Filled.Visibility;
                _passwordInput = InputType.Text;
            }
        }

        private void FillAdministratorCredentials()
        {
            _tokenModel.Email = ApplicationConstants.Defaults.Users.Administrators.FirstOrDefault()?.Email;
            _tokenModel.Password = ApplicationConstants.Defaults.Users.Administrators.FirstOrDefault()?.Password;
        }

        private void FillBasicUserCredentials()
        {
            _tokenModel.Email = ApplicationConstants.Defaults.Users.Basic.FirstOrDefault()?.Email;
            _tokenModel.Password = ApplicationConstants.Defaults.Users.Basic.FirstOrDefault()?.Password;
        }

        private string GetEmailLabel()
        {
            if(_config?.ServerConfiguration?.LoginSettings?.AllowLoginWithUsername == true)
                return _localizer["Username or Email"];
            return _localizer["E-mail"];
        }
    }
}