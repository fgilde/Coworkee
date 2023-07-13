using Coworkee.Application.Requests.Identity;
using MudBlazor;
using System.Text;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using Coworkee.Client.Extensions;
using System;

namespace Coworkee.Client.Pages.Identity
{
    public partial class Reset
    {
        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private readonly ResetPasswordRequest _resetPasswordModel = new();

        protected override void OnInitialized()
        {
            var queryToken = _navigationManager.ReadQueryParam("Token");
            if (!string.IsNullOrEmpty(queryToken))
            {
                //_resetPasswordModel.Token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(queryToken));
                queryToken = queryToken.Replace('-', '+').Replace('_', '/');
                _resetPasswordModel.Token = Encoding.UTF8.GetString(Convert.FromBase64String(queryToken));
            }
        }


        private async Task SubmitAsync()
        {
            if (!string.IsNullOrEmpty(_resetPasswordModel.Token))
            {
                var result = await _api.User_ResetPasswordAsync(_resetPasswordModel);
                if (_errorService.IsSuccessFull(result))
                {
                    _snackBar.Add(result.Messages[0], Severity.Success);
                    _navigationManager.NavigateTo("/");
                }
            }
            else
            {
                _snackBar.Add(_localizer["Token Not Found!"], Severity.Error);
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
    }
}