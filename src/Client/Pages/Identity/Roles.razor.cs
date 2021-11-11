using CleanArchitectureBase.Application.Responses.Identity;
using CleanArchitectureBase.Client.Extensions;
using CleanArchitectureBase.Shared.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Requests.Identity;
using CleanArchitectureBase.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;

namespace CleanArchitectureBase.Client.Pages.Identity
{
    public partial class Roles
    {
        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private List<RoleResponse> _roleList = new();
        private RoleResponse _role = new();
        private string _searchString = "";
        private bool _dense = false;
        private bool _striped = true;
        private bool _bordered = false;

        private ClaimsPrincipal _currentUser;
        private bool _canCreateRoles;
        private bool _canEditRoles;
        private bool _canDeleteRoles;
        private bool _canSearchRoles;
        private bool _canViewRoleClaims;
        private bool _loaded;

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _clientAuthenticationManager.CurrentUser();
            _canCreateRoles = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Roles.Create)).Succeeded;
            _canEditRoles = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Roles.Edit)).Succeeded;
            _canDeleteRoles = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Roles.Delete)).Succeeded;
            _canSearchRoles = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Roles.Search)).Succeeded;
            _canViewRoleClaims = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.RoleClaims.View)).Succeeded;

            await GetRolesAsync();
            _loaded = true;
            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        private async Task GetRolesAsync()
        {
            var response = await _api.Role_GetAllAsync();
            if(_errorService.IsSuccessFull(response))
                _roleList = response.Data.ToList();
        }

        private async Task Delete(string id)
        {
            string deleteContent = _localizer["Delete Role Content"];
            var parameters = new DialogParameters
            {
                {nameof(Shared.Dialogs.DeleteConfirmation.Message), string.Format(deleteContent, id)}
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<Shared.Dialogs.DeleteConfirmation>(_localizer["Delete"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                var response = await _api.Role_DeleteAsync(id);
                if (_errorService.IsSuccessFull(response))
                {
                    await Reset();
                    await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
                    _snackBar.Add(response.Messages[0], Severity.Success);
                }
            }
        }

        private async Task InvokeModal(string id = null)
        {
            var parameters = new DialogParameters();
            if (id != null)
            {
                _role = _roleList.FirstOrDefault(c => c.Id == id);
                if (_role != null)
                {
                    parameters.Add(nameof(RoleModal.RoleModel), new RoleRequest
                    {
                        Id = _role.Id,
                        Name = _role.Name,
                        Description = _role.Description
                    });
                }
            }
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<RoleModal>(id == null ? _localizer["Create"] : _localizer["Edit"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await Reset();
            }
        }

        private async Task Reset()
        {
            _role = new RoleResponse();
            await GetRolesAsync();
        }

        private bool Search(RoleResponse role)
        {
            return string.IsNullOrWhiteSpace(_searchString) ||
                   (role.Name?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                    role.Description?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true);
        }

        private void ManagePermissions(string roleId)
        {
            _navigationManager.NavigateTo($"/identity/role-permissions/{roleId}");
        }
    }
}