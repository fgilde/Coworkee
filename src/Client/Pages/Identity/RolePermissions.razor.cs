using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Coworkee.Application.Requests.Identity;
using Coworkee.Client.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System.Threading.Tasks;
using Coworkee.Application.Common.Models.Identity;
using Coworkee.Application.Hubs;
using Coworkee.Client.Shared.Dialogs;
using Coworkee.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Options;
using Nextended.Core.Extensions;

namespace Coworkee.Client.Pages.Identity
{
    public partial class RolePermissions: IAsyncDisposable
    {

        [CascadingParameter] private HubConnection HubConnection { get; set; }
        [Parameter] public string Id { get; set; }
        [Parameter] public string Title { get; set; }
        [Parameter] public string Description { get; set; }

        private PermissionResponse _model;
        private Dictionary<string, List<RoleClaimResponse>> GroupedRoleClaims { get; } = new();
        private RoleClaimResponse _roleClaims = new();
        private RoleClaimResponse _selectedItem = new();
        private string _searchString = "";

        private ClaimsPrincipal _currentUser;
        private bool _canEditRolePermissions;
        private bool _canSearchRolePermissions;
        private bool _loaded;

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _clientAuthenticationManager.CurrentUser();
            _canEditRolePermissions = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.RoleClaims.Edit)).Succeeded;
            _canSearchRolePermissions = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.RoleClaims.Search)).Succeeded;

            await GetRolePermissionsAsync();
            _loaded = true;
            HubConnection = await HubConnection.EnsureStartedAsync(_config.BackendOrigin);
        }

        private async Task GetRolePermissionsAsync()
        {
            var roleId = Id;
            var result = await _api.Role_GetPermissionsByRoleIdAsync(roleId);
            if (result.Succeeded)
            {
                _model = result.Data;
                GroupedRoleClaims.Add(_localizer["All Permissions"], _model.RoleClaims);
                foreach (var claim in _model.RoleClaims)
                {
                    if (GroupedRoleClaims.ContainsKey(claim.Group))
                    {
                        GroupedRoleClaims[claim.Group].Add(claim);
                    }
                    else
                    {
                        GroupedRoleClaims.Add(claim.Group, new List<RoleClaimResponse> { claim });
                    }
                }
                if (_model != null)
                {
                    Description = string.Format(_localizer["Manage {0} {1}'s Permissions"], _model.RoleId, _model.RoleName);
                }
            }
            else
            {
                foreach (var error in result.Messages)
                {
                    _snackBar.Add(error, Severity.Error);
                }
                _navigationManager.NavigateTo("/identity/roles");
            }
        }

        private async Task SaveAsync()
        {
            var request = _model.MapTo<PermissionRequest>();
            var result = await _api.Role_UpdateAsync(request);
            if (result.Succeeded)
            {
                _snackBar.Add(result.Messages[0], Severity.Success);
                await HubConnection.TrySendAsync(_config.BackendOrigin, nameof(ClientEventHub.RegenerateTokensAsync));
                _navigationManager.NavigateTo("/identity/roles");
            }
            else
            {
                foreach (var error in result.Messages)
                {
                    _snackBar.Add(error, Severity.Error);
                }
            }
        }

        private bool Search(RoleClaimResponse roleClaims)
        {
            return string.IsNullOrWhiteSpace(_searchString) ||
                   (roleClaims.ClaimValue?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                    roleClaims.Description?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true);
        }

        private Color GetGroupBadgeColor(int selected, int all)
        {
            return selected == 0 ? Color.Error : selected == all ? Color.Success : Color.Info;
        }

        private async void CheckChange(RoleClaimResponse context, bool isChecked)
        {
            if (isChecked)
            {
                var requiredDependencies = Permissions.GetRequiredDependencyPermissionsFor(context.ClaimValue);
                var neededAdditionalPermissions = GroupedRoleClaims.Values.SelectMany(l => l).Where(roleClaim => requiredDependencies.Contains(roleClaim.ClaimValue) && !roleClaim.Selected).Distinct().ToArray();
                if (neededAdditionalPermissions.Any())
                {
                    string message = neededAdditionalPermissions.Length == 1
                        ? _localizer["The following permission is also required for permission {1}"]
                        : _localizer["The following {0} permissions are also required for permission {1}"];
                    var parameters = new DialogParameters
                    {
                        {nameof(PermissionsRequired.RequiredClaims), neededAdditionalPermissions},
                        {nameof(PermissionsRequired.Message), string.Format(message, neededAdditionalPermissions.Length, _localizer[context.ClaimValue])}
                    };
                    var options = new DialogOptionsEx { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, FullScreen = false, BackdropClick = false };
                    var dialog = await _dialogService.ShowEx<PermissionsRequired>(_localizer["Dependent permissions required"], parameters, options);
                    var result = await dialog.Result;
                    if (!result.Canceled)
                    {
                        if (result.Data.MapTo<bool>())
                            neededAdditionalPermissions.Apply(r => r.Selected = true);
                    }
                    else
                    {
                        context.Selected = false;
                        return;
                    }

                    StateHasChanged();
                }
            }
            context.Selected = isChecked;
        }

        public ValueTask DisposeAsync()
        {
            return HubConnection.TryDisposeAsync();
        }
    }
}