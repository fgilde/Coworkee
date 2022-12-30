using System.Collections.Generic;
using System.Linq;
using Coworkee.Application.Requests.Identity;
using MudBlazor;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components.Web;
using Coworkee.Application.Common.Models.Identity;
using Coworkee.Client.Shared.Components;
using Coworkee.Shared.Constants.Application;

namespace Coworkee.Client.Pages.Authentication
{
    public partial class Register
    {
        public int SelectedPageIndex
        {
            get => _selectedPageIndex;
            set
            {
                _selectedPageIndex = value;
                if (!_visitedPages.Contains(value))
                    _visitedPages.Add(value);
            }
        }
        public bool AllPagesVisited => Enumerable.Range(0, _pages.Count).All(_visitedPages.Contains);


        private UploadRequestEdit _uploadEdit;
        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private RegisterRequest _registerUserModel = new() { UserInfo = new UserInformationsDto() };
        private AddressDto _address => _registerUserModel.UserInfo.Addresses.FirstOrDefault();
        private List<RoleDto> _selectableRoles;
        private bool _processing;

        private bool _passwordVisibility;
        private InputType _passwordInput = InputType.Password;
        private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

        private readonly List<string> _pages = new();
        private readonly List<int> _visitedPages = new();
        private int _selectedPageIndex;

        protected override async Task OnInitializedAsync()
        {
            if (!_config.ServerConfiguration.UserRegistration.Enabled)
            {
                _navigationManager.NavigateTo(ApplicationConstants.Routes.Login);
            }
            else
            {
                if (_config.ServerConfiguration.UserRegistration.RequireAddress)
                    _registerUserModel.UserInfo.Addresses = new List<AddressDto> { new() };

                _selectableRoles = (await _api.Role_GetPublicRolesAsync()).Data;

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
                    _snackBar.Add(_localizer[response.Messages[0]], Severity.Success);
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

        private void AddPage(string name)
        {
            if (!_pages.Contains(name))
            {
                _pages.Add(name);
                StateHasChanged();
            }
        }

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

        private bool IsAddedAsInitialRole(RoleDto role)
        {
            return _registerUserModel.InitialRoleNames.Contains(role.Name);
        }

        private void ToggleRole(RoleDto role)
        {
            if (IsAddedAsInitialRole(role))
                _registerUserModel.InitialRoleNames.Remove(role.Name);
            else
                _registerUserModel.InitialRoleNames.Add(role.Name);
            StateHasChanged();
        }

        private async Task NavigateToNextPageOrPost(KeyboardEventArgs args)
        {
            if (args.Key == "Enter")
            {
                if (!Validated && SelectedPageIndex < _pages.Count - 1)
                    SelectedPageIndex++;
                else if (Validated)
                    await SubmitAsync();
            }
        }
    }
}