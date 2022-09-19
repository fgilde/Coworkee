using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Contracts.Enums;
using CleanArchitectureBase.Application.Requests;
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


        private async Task<PaginatedResult<ProductDto>> Load(int pageNumber, int pageSize, string _searchString, string[] orderings, CancellationToken cancellationToken)
        {
            return await _api.Products_GetAllAsync(pageNumber, pageSize, _searchString, orderings, cancellationToken: cancellationToken);
        }

        private async Task<ProductDto> FindById(string id, IEnumerable<ProductDto> loaded)
        {
            return loaded.FirstOrDefault(p => p.Id == id) ?? await _api.Products_GetByIdAsync(id);
        }

        private string GetId(ProductDto product)
        {
            return product.Id;
        }

        private async Task<Result> DeleteProducts(string[] ids)
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
            await (await _api.Products_ExportAsync(exportServiceType: exportServiceType, searchString: search)).ForceDownloadAsync(_jsRuntime);
        }

        private async Task ExportSelected(ExportServiceType exportServiceType, string[] ids)
        {
            await (await _api.Products_ExportAsync(exportServiceType: exportServiceType, ids: ids)).ForceDownloadAsync(_jsRuntime);
        }


        private async Task<bool> CreateOrEditProduct(ProductDto productOrNull)
        {
            if (!string.IsNullOrWhiteSpace(productOrNull?.ImageDataURL) && productOrNull.UploadRequest == null)
                productOrNull.UploadRequest = await UploadRequest.FromUrlAsync(_navigationManager.ToAbsoluteServerUri(productOrNull.ImageDataURL));
            return !(await _dialogService.EditOrCreate(productOrNull, async (dto, _) =>
            {
                dto.ImageDataURL = string.Empty; // Is overridden by UploadRequest
                await _api.Products_PostAsync(new(dto));
                return null;
            })).Cancelled;

            //var parameters = new DialogParameters();
            //if (productOrNull != null)
            //{
            //    parameters.Add(nameof(AddEditProductModal.AddEditProductModel), productOrNull);
            //}

            //var dialog = await _dialogService.ShowWithDefaultOptionsAsync<AddEditProductModal>(productOrNull == null ? _localizer["Create"] : _localizer["Edit"], parameters);
            //var result = await dialog.Result;
            //return !result.Cancelled;
        }
    }
}