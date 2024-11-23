using MudBlazor;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Coworkee.Application.Common.Models.Identity;
using Coworkee.Client.Extensions;
using Coworkee.Client.JsInterop;
using Coworkee.Shared.Constants.Application;
using Coworkee.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.JSInterop;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Options;

namespace Coworkee.Client.Pages.Identity
{
    public partial class Users
    {
        private List<UserResponse> _userList = new();
        private UserResponse _user = new();
        private string _searchString = "";

        private ClaimsPrincipal _currentUser;
        private bool _canCreateUsers;
        private bool _canSearchUsers;
        private bool _canExportUsers;
        private bool _canDeleteUsers;
        private bool _canViewRoles;
        private bool _loaded;

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _clientAuthenticationManager.CurrentUser();
            _canCreateUsers = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Users.Create)).Succeeded;
            _canSearchUsers = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Users.Search)).Succeeded;
            _canExportUsers = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Users.Export)).Succeeded;
            _canDeleteUsers = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Users.Delete)).Succeeded;
            _canViewRoles = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Roles.View)).Succeeded;

            await GetUsersAsync();
            _loaded = true;
        }

        private async Task GetUsersAsync()
        {
            var response = await _api.User_GetAllAsync();
            if (_errorService.IsSuccessFull(response))
                _userList = response.Data.ToList();

        }

        private bool Search(UserResponse user)
        {
            return string.IsNullOrWhiteSpace(_searchString)
                   || user.FirstName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true
                   || user.LastName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true
                   || user.Email?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true
                   || user.PhoneNumber?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true
                   || user.UserName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true
                   || user.CreatedOn.AsClientLocalTime().ToString("G", CultureInfo.CurrentCulture)?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true;
        }

        private async Task ExportToExcel()
        {
            var base64 = await _api.User_ExportAsync(_searchString);
            await _jsRuntime.InvokeVoidAsync(JsNamespace.Get("BrowserHelper", "download"), new
            {
                Base64String = base64,
                FileName = $"{nameof(Users).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
                MimeType = ApplicationConstants.MimeTypes.OpenXml
            });
            _snackBar.Add(string.IsNullOrWhiteSpace(_searchString)
                ? _localizer["Users exported"]
                : _localizer["Filtered Users exported"], Severity.Success);
        }

        private async Task InvokeModal()
        {
            var parameters = new DialogParameters();
            var dialog = await _dialogService.ShowWithDefaultOptionsAsync<RegisterUserModal>(_localizer["Register New User"], parameters);
            if (!(await dialog.Result).Canceled)
                await GetUsersAsync();
        }

        private void ViewProfile(string userId)
        {
            _navigationManager.NavigateTo($"/user-profile/{userId}");
        }

        private async void DeleteUser(UserResponse user)
        {
            var parameters = new DialogParameters
            {
                {nameof(Shared.Dialogs.DeleteConfirmation.Message), $"{string.Format(_localizer["Do you want to delete the User {0}"], user.FullName)}?"}
            };
            var options = new DialogOptionsEx { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, BackdropClick = false, Animations = DialogServiceExtensions.DefaultAnimationNoFullHeight };
            var dialog = await _dialogService.ShowEx<Shared.Dialogs.DeleteConfirmation>(_localizer["Delete"], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled && _errorService.IsSuccessFull(await _api.User_DeleteAsync(user.Id)))
            {
                _snackBar.Add(_localizer["User Deleted"], Severity.Success);
                await GetUsersAsync();
                StateHasChanged();
            }
        }

        private void ManageRoles(string userId, string email)
        {
            if (email == ApplicationConstants.Defaults.Users.System.Email || ApplicationConstants.Defaults.Users.Administrators.Any(u => u.Email == email)) _snackBar.Add(_localizer["Not Allowed."], Severity.Error);
            else _navigationManager.NavigateTo($"/identity/user-roles/{userId}");
        }
    }
}