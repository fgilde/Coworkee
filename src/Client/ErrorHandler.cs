using CleanArchitectureBase.Shared.Wrapper;
using MudBlazor;

namespace CleanArchitectureBase.Client
{
    public class ErrorHandler
    {
        private readonly ISnackbar _snackbar;

        public ErrorHandler(ISnackbar snackbar)
        {
            _snackbar = snackbar;
        }

        public void ShowError(params string[] errors)
        {
            foreach (var message in errors)
                _snackbar.Add(message, Severity.Error);
        }

        public bool EnsureResultSuccess(IResult result)
        {
            if (!result.Succeeded)
            {
                ShowError(result.Messages.ToArray());
                return false;
            }

            return true;
        }
    }
}