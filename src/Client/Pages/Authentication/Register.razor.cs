using CleanArchitectureBase.Application.Requests.Identity;
using MudBlazor;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using CleanArchitectureBase.Application.Configurations;
using CleanArchitectureBase.Client.JsInterop;
using CleanArchitectureBase.Client.Shared.Components;
using CleanArchitectureBase.Shared.Constants.Application;

namespace CleanArchitectureBase.Client.Pages.Authentication
{
    public partial class Register
    {
        private UploadRequestEdit _uploadEdit;
        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private RegisterRequest _registerUserModel = new();
        private bool _processing;

        protected override async Task OnInitializedAsync()
        {
            if (!_config.ServerConfiguration.UserRegistration.Enabled)
            {
                _navigationManager.NavigateTo(ApplicationConstants.Routes.Login);
            }
            else
            {
                await base.OnInitializedAsync();
            }
        }

        private async Task SubmitAsync()
        {
            try
            {
                _processing = true;
                var response = await _api.User_RegisterAsync(_registerUserModel);
                if (_errorService.IsSuccessFull(response))
                {
                    _snackBar.Add(response.Messages[0], Severity.Success);
                    _navigationManager.NavigateTo(ApplicationConstants.Routes.Login);
                    _registerUserModel = new RegisterRequest();
                }
            }
            finally
            {
                _processing = false;
                StateHasChanged();
            }
        }

        private bool _passwordVisibility;
        private InputType _passwordInput = InputType.Password;
        private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

        private void TogglePasswordVisibility()
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

        private Task Upload(MouseEventArgs arg)
        {
            return _uploadEdit.Upload(arg);
        }
    }
}