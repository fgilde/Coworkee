using lib.Coworkee.Application.Requests.Identity;
using MudBlazor;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using Coworkee.Client.Extensions;
using lib.Coworkee.Shared.Constants.Application;

namespace Coworkee.Client.Pages.Identity
{
    public partial class Forgot
    {
        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private readonly ForgotPasswordRequest _emailModel = new();
        private bool _isEmailConfirm;

        protected override Task OnInitializedAsync()
        {
            _emailModel.Email = _navigationManager.ReadQueryParam("email");
            _isEmailConfirm = _navigationManager.ReadQueryParam("email-confirm") == "true";
            return base.OnInitializedAsync();
        }

        private async Task SubmitAsync()
        {
            var result = await _api.User_ForgotPasswordAsync(_emailModel);
            if (_errorService.IsSuccessFull(result))
            {
                _snackBar.Add(_localizer["Done!"], Severity.Success);
                foreach (var message in result.Messages)
                {
                    _snackBar.Add(message, Severity.Success);
                }
                _navigationManager.NavigateTo($"{ApplicationConstants.Routes.Login}?email={_emailModel.Email}");
            }
        }
    }
}