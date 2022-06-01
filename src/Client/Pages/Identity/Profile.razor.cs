using CleanArchitectureBase.Application.Requests.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System;
using System.IO;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using Microsoft.JSInterop;
using CleanArchitectureBase.Application.Common.Extensions;
using CleanArchitectureBase.Application.Contracts.Enums;
using CleanArchitectureBase.Shared.Constants.Storage;

namespace CleanArchitectureBase.Client.Pages.Identity
{
    public partial class Profile : IAsyncDisposable
    {
        [Parameter] public string ImageDataUrl { get; set; }

        public string UserId { get; set; }


        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private readonly UpdateProfileRequest _profileModel = new();
        private ElementReference dropZoneElement;
        private InputFile inputFile;
        private IJSObjectReference _module;
        private IJSObjectReference _dropZoneInstance;

        private async Task UpdateProfileAsync()
        {
            var token = await _api.Account_UpdateProfileAsync(_profileModel);
            await _localStorage.SetItemAsync(StorageConstants.Local.AuthToken, token);
            _snackBar.Add(_localizer["Your Profile has been updated."], Severity.Success);
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
            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            _profileModel.Email = user.GetEmail();
            _profileModel.FirstName = user.GetFirstName();
            _profileModel.LastName = user.GetLastName();
            _profileModel.PhoneNumber = user.GetPhoneNumber();
            UserId = user.GetUserId();
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
                await _localStorage.SetItemAsync(StorageConstants.Local.UserImageURL, result.Data.UserImageURL);
                await _localStorage.SetItemAsync(StorageConstants.Local.AuthToken, result.Data.Token);
                ImageDataUrl = result.Data.UserImageURL;
                StateHasChanged();
                _snackBar.Add(_localizer["Profile picture added."], Severity.Success);
            }
        }

        private async Task DeleteAsync()
        {
            var parameters = new DialogParameters
            {
                {nameof(Shared.Dialogs.DeleteConfirmation.Message), $"{string.Format(_localizer["Do you want to delete the profile picture of {0}"], _profileModel.Email)}?"}
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<Shared.Dialogs.DeleteConfirmation>(_localizer["Delete"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                var request = new UpdateProfilePictureRequest { Data = null, FileName = string.Empty, UploadType = UploadType.ProfilePicture };
                var data = await _api.Account_UpdateProfilePictureAsync(request, UserId);
                if (_errorService.IsSuccessFull(data))
                {
                    await _localStorage.SetItemAsync(StorageConstants.Local.UserImageURL, string.Empty);
                    await _localStorage.SetItemAsync(StorageConstants.Local.AuthToken, data.Data.Token);
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
    }
}