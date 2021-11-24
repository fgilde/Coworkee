using System.Linq;
using CleanArchitectureBase.Application.Requests.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using CleanArchitectureBase.Client.Authentication;
using CleanArchitectureBase.Client.Extensions;
using CleanArchitectureBase.Shared.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Client.Pages.Authentication
{
    public partial class Login
    {
        //[Parameter]
        //public string ReturnUrl { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private TokenRequest _tokenModel = new();

        protected override async Task OnInitializedAsync()
        {
            var state = await _stateProvider.GetAuthenticationStateAsync();
            if (state != AuthenticationStates.None && !state.IsGuest())
            {
                if (state?.User.Identity?.IsAuthenticated == true)
                {
                    _navigationManager.NavigateTo("/forbidden/"+ _navigationManager.ToBaseRelativePath(_navigationManager.Uri));
                }
                else
                {
                    _navigationManager.NavigateToHomeWithReturnTo();
                }
            }
        }

        private async Task SubmitAsync()
        {
            var result = await _clientAuthenticationManager.Login(_tokenModel);
            if (_errorService.IsSuccessFull(result))
            {
                _snackBar.Add(string.Format(_localizer["Welcome {0}"], _tokenModel.Email), Severity.Success);
                _navigationManager.NavigateToReturnUrlIf("/");
            }
        }

        private bool _passwordVisibility;
        private InputType _passwordInput = InputType.Password;
        private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

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
            _tokenModel.Email = ApplicationConstants.Defaults.DefaultAdminUserEmail;
            _tokenModel.Password = ApplicationConstants.Defaults.DefaultAdminUserPassword;
        }

        private void FillBasicUserCredentials()
        {
            _tokenModel.Email = ApplicationConstants.Defaults.DefaultBasicUserEmail;
            _tokenModel.Password = ApplicationConstants.Defaults.DefaultBasicUserPassword;
        }
    }
}