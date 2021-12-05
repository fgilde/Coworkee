using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Contracts.Enums;
using CleanArchitectureBase.Client.Extensions;
using CleanArchitectureBase.Shared.Wrapper;

namespace CleanArchitectureBase.Client.Pages.Catalog
{
    public partial class Products
    {
        
        [Parameter]
        public string Action { get; set; }

        [Parameter]
        public string Id { get; set; }

        
        private async Task<PaginatedResult<ProductDto>> Load(int pageNumber, int pageSize, string _searchString, string[] orderings)
        {
            return await _api.Products_GetAllAsync(pageNumber, pageSize, _searchString, orderings);
        }

        private async Task<ProductDto> FindById(int id, IEnumerable<ProductDto> loaded)
        {
            return loaded.FirstOrDefault(p => p.Id == id) ?? await _api.Products_GetByIdAsync(id);
        }

        private int GetId(ProductDto product)
        {
            return product.Id;
        }

        private async Task<Result> DeleteProducts(int[] ids)
        {
            await _api.Products_DeleteAsync(ids.ToList());
            return await Result.SuccessAsync(_localizer["Product Deleted"]) as Result;
        }

        private string GetName(ProductDto arg)
        {
            return arg.Name;
        }

        private async Task Export(ExportServiceType exportServiceType, string search)
        {
            await (await _api.Products_ExportAsync(exportServiceType, search)).ForceDownloadAsync(_jsRuntime);
        }

        private async Task ExportSelected(ExportServiceType exportServiceType, int[] ids)
        {
            await (await _api.Products_ExportAsync(exportServiceType, ids: ids)).ForceDownloadAsync(_jsRuntime);
        }


        private async Task<bool> CreateOrEditProduct(ProductDto productOrNull)
        {
            var parameters = new DialogParameters();
            if (productOrNull != null)
            {
                parameters.Add(nameof(AddEditProductModal.AddEditProductModel), productOrNull);
            }

            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };
            var dialog = await _dialogService.ShowAsync<AddEditProductModal>(productOrNull == null ? _localizer["Create"] : _localizer["Edit"], parameters, options);
            var result = await dialog.Result;
            return !result.Cancelled;
        }
    }
}