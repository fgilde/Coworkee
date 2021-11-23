using CleanArchitectureBase.Application.Requests.Identity;
using CleanArchitectureBase.Client.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System;
using System.IO;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using CleanArchitectureBase.Application.Contracts.Enums;
using CleanArchitectureBase.Client.Shared.Components;
using CleanArchitectureBase.Shared.Constants.Storage;

namespace CleanArchitectureBase.Client.Pages.Identity
{
    public partial class Profile
    {
        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        
        private readonly UpdateProfileRequest _profileModel = new();
        private UserAvatar avatar;
        public string UserId { get; set; }

        private async Task UpdateProfileAsync()
        {
            var token = await _api.Account_UpdateProfileAsync(_profileModel);
            await _localStorage.SetItemAsync(StorageConstants.Local.AuthToken, token);
            _snackBar.Add(_localizer["Your Profile has been updated."], Severity.Success);
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

        private IBrowserFile _file;

        [Parameter]
        public string ImageDataUrl { get; set; }

        private async Task UploadFiles(InputFileChangeEventArgs e)
        {
            _file = e.File;
            if (_file != null)
            {
                var extension = Path.GetExtension(_file.Name);
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
                    avatar.SetImageDataUrl(ImageDataUrl);
                    StateHasChanged();
                    _snackBar.Add(_localizer["Profile picture added."], Severity.Success);
                }
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
                    avatar.SetImageDataUrl(ImageDataUrl);
                    StateHasChanged();
                    _snackBar.Add(_localizer["Profile picture deleted."], Severity.Success);
                }
            }
        }
    }
}