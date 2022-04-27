using CleanArchitectureBase.Application.Features.Products.Commands.AddEdit;
using CleanArchitectureBase.Application.Requests;
using CleanArchitectureBase.Client.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Contracts.Enums;
using CleanArchitectureBase.Application.Hubs;

namespace CleanArchitectureBase.Client.Pages.Catalog
{
    public partial class AddEditProductModal: IAsyncDisposable
    {
        [Parameter] public ProductDto AddEditProductModel { get; set; } = new();
        [CascadingParameter] private HubConnection HubConnection { get; set; }
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private IList<BrandDto> _brands = new List<BrandDto>();

        public void Cancel()
        {
            MudDialog.Cancel();
        }

        private async Task SaveAsync()
        {
            await _api.Products_PostAsync(new AddEditProductsCommand(AddEditProductModel));
            
            _snackBar.Add(_localizer["Product Updated"], Severity.Success);
            await HubConnection.SendAsync(nameof(ClientEventHub.UpdateDashboardAsync));
            MudDialog.Close();
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
            HubConnection = await HubConnection.EnsureStartedAsync(_config.BackendOrigin);
        }

        private async Task LoadDataAsync()
        {
            if(!AddEditProductModel.IsNew)
                await LoadImageAsync();
            await LoadBrandsAsync();
        }

        private async Task LoadBrandsAsync()
        {
            _brands = await _api.Brands_GetAllAsync();
        }

        private async Task LoadImageAsync()
        {
            var data = await _api.Products_GetProductImageAsync(AddEditProductModel.Id);
            if (data.Succeeded)
            {
                var imageData = data.Data;
                if (!string.IsNullOrEmpty(imageData))
                {
                    AddEditProductModel.ImageDataURL = imageData;
                }
            }
        }

        private void DeleteAsync()
        {
            AddEditProductModel.ImageDataURL = null;
            AddEditProductModel.UploadRequest = new UploadRequest();
        }

        private IBrowserFile _file;

        private async Task UploadFiles(InputFileChangeEventArgs e)
        {
            _file = e.File;
            if (_file != null)
            {
                
                var extension = Path.GetExtension(_file.Name);
                var format = "image/png";
                var imageFile = await e.File.RequestImageFileAsync(format, 400, 400);
                var buffer = new byte[imageFile.Size];
                await imageFile.OpenReadStream().ReadAsync(buffer);
                AddEditProductModel.ImageDataURL = $"data:{format};base64,{Convert.ToBase64String(buffer)}";
                AddEditProductModel.UploadRequest = new UploadRequest { Data = buffer, FileName = _file.Name ,UploadType = UploadType.Product, Extension = extension };
            }
        }

        private async Task<IEnumerable<string>> SearchBrands(string value)
        {
            // In real life use an asynchronous function for fetching data from an api.
            await Task.Delay(5);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return _brands.Select(x => x.Id);

            return _brands.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.Id);
        }

        public ValueTask DisposeAsync()
        {
            return HubConnection.TryDisposeAsync();
        }
    }
}