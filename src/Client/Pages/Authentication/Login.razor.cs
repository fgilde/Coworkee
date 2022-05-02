using System.Linq;
using CleanArchitectureBase.Application.Requests.Identity;
using MudBlazor;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using CleanArchitectureBase.Client.Extensions;
using CleanArchitectureBase.Shared.Constants.Application;

namespace CleanArchitectureBase.Client.Pages.Authentication
{
    public partial class Login
    {
        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private TokenRequest _tokenModel = new();
        
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
            _tokenModel.Email = ApplicationConstants.Defaults.Users.Administrators.FirstOrDefault()?.Email;
            _tokenModel.Password = ApplicationConstants.Defaults.Users.Administrators.FirstOrDefault()?.Password;
        }

        private void FillBasicUserCredentials()
        {
            _tokenModel.Email = ApplicationConstants.Defaults.Users.Basic.FirstOrDefault()?.Email;
            _tokenModel.Password = ApplicationConstants.Defaults.Users.Basic.FirstOrDefault()?.Password;
        }
    }
}