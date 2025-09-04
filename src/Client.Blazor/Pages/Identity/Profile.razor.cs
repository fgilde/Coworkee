using Coworkee.Application.Requests.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.JSInterop;
using Coworkee.Application.Common.Extensions;
using Coworkee.Application.Common.Models.Identity;
using Coworkee.Application.Contracts.Enums;
using Coworkee.Client.Extensions;
using Coworkee.Shared.Constants.Permission;
using Coworkee.Shared.Constants.Storage;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Options;

namespace Coworkee.Client.Pages.Identity
{
    public partial class Profile : IAsyncDisposable
    {
        [Parameter] public string ImageDataUrl { get; set; }
        [Parameter] public string UserId { get; set; }
        [Parameter] public Variant Variant { get; set; } = Variant.Text;

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

        private UserResponse user;
        private ClaimsPrincipal _currentUser;
        //private readonly UpdateProfileRequest _profileModel = new(); // TODO: remove

        private bool _canEditUser;
        private bool _canEditUserAsAdmin;

        private ElementReference dropZoneElement;
        private InputFile inputFile;
        private IJSObjectReference _module;
        private IJSObjectReference _dropZoneInstance;

        private async Task UpdateProfileAsync()
        {
            var result = await _api.User_UpdateProfileAsync(user);
            if (_errorService.IsSuccessFull(result))
            {
                _snackBar.Add(_localizer["Profile updated"], Severity.Success);
                if (UserId == _currentUser.GetUserId() && !string.IsNullOrWhiteSpace(result.Data))
                    await _localStorage.SetItemAsync(StorageConstants.Local.AuthToken, result.Data);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/helper/dropZone.js");
                _dropZoneInstance = await _module.InvokeAsync<IJSObjectReference>("initializeFileDropZone", dropZoneElement, inputFile.Element);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            _currentUser = await _clientAuthenticationManager.CurrentUser();
            UserId = string.IsNullOrEmpty(UserId) ? _currentUser.GetUserId() : UserId;
            user = (await _api.User_GetByIdAsync(UserId)).Data;
            _canEditUserAsAdmin = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Users.Edit)).Succeeded;
            _canEditUser = UserId == _currentUser.GetUserId() || _canEditUserAsAdmin;

            var data = await _api.Account_GetProfilePictureAsync(UserId);
            if (data.Succeeded)
            {
                ImageDataUrl = data.Data;
            }
        }


        private async Task UploadFiles(InputFileChangeEventArgs e)
        {
            var file = e.File;
            var extension = Path.GetExtension(file.Name);
            var fileName = $"{UserId}-{Guid.NewGuid()}{extension}";
            var format = "image/png";
            var imageFile = await e.File.RequestImageFileAsync(format, 400, 400);
            var buffer = new byte[imageFile.Size];
            await imageFile.OpenReadStream().ReadAsync(buffer);
            var request = new UpdateProfilePictureRequest { Data = buffer, FileName = fileName, Extension = extension, UploadType = UploadType.ProfilePicture };
            var result = await _api.Account_UpdateProfilePictureAsync(request, UserId);
            if (_errorService.IsSuccessFull(result))
            {
                if (UserId == _currentUser.GetUserId())
                {
                    await _localStorage.SetItemAsync(StorageConstants.Local.UserImageURL, result.Data.UserImageURL);
                    await _localStorage.SetItemAsync(StorageConstants.Local.AuthToken, result.Data.Token);
                }

                ImageDataUrl = result.Data.UserImageURL;
                StateHasChanged();
                _snackBar.Add(_localizer["Profile picture added."], Severity.Success);
            }
        }

        private async Task DeleteAsync()
        {
            var parameters = new DialogParameters
            {
                {nameof(Shared.Dialogs.DeleteConfirmation.Message), $"{string.Format(_localizer["Do you want to delete the profile picture of {0}"], user.Email)}?"}
            };
            var options = new DialogOptionsEx { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, BackdropClick = false, Animations = DialogServiceExtensions.DefaultAnimationNoFullHeight};
            var dialog = await _dialogService.ShowEx<Shared.Dialogs.DeleteConfirmation>(_localizer["Delete"], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                var request = new UpdateProfilePictureRequest { Data = null, FileName = string.Empty, UploadType = UploadType.ProfilePicture };
                var data = await _api.Account_UpdateProfilePictureAsync(request, UserId);
                if (_errorService.IsSuccessFull(data))
                {
                    if (UserId == _currentUser.GetUserId())
                    {
                        await _localStorage.SetItemAsync(StorageConstants.Local.UserImageURL, string.Empty);
                        await _localStorage.SetItemAsync(StorageConstants.Local.AuthToken, data.Data.Token);
                    }

                    ImageDataUrl = string.Empty;
                    StateHasChanged();
                    _snackBar.Add(_localizer["Profile picture deleted."], Severity.Success);
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_dropZoneInstance != null)
            {
                await _dropZoneInstance.InvokeVoidAsync("dispose");
                await _dropZoneInstance.DisposeAsync();
            }

            if (_module != null)
                await _module.DisposeAsync();

        }

        private void OnAddressCreated(AddressDto obj)
        {
            user.UserInfo ??= new UserInformationsDto();
            user.UserInfo.Addresses ??= new List<AddressDto>();
            user.UserInfo.Addresses.Add(obj);
        }
    }
}