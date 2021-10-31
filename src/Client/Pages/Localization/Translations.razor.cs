using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Dtos;
using CleanArchitectureBase.Shared.Wrapper;

namespace CleanArchitectureBase.Client.Pages.Localization
{
    public partial class Translations
    {
        
        [Parameter]
        public string Action { get; set; }

        [Parameter]
        public string Id { get; set; }

        
        private async Task<PaginatedResult<TranslationDto>> Load(int pageNumber, int pageSize, string _searchString, string[] orderings)
        {
            return await _api.Translations_GetAllAsync(pageNumber, pageSize, _searchString, orderings);
        }

        private async Task<TranslationDto> FindById(int id, IEnumerable<TranslationDto> loaded)
        {
            var res = loaded.FirstOrDefault(p => p.Id == id) ?? (await _api.Translations_GetByIdAsync(id))?.Data;
            return res;
        }

        private int GetId(TranslationDto translation)
        {
            return translation.Id;
        }

        private async Task<Result> DeleteTranslations(int[] ids)
        {
            return await _api.Translations_DeleteAsync(ids.ToList());
        }

        private string GetName(TranslationDto arg)
        {
            return arg.Key;
        }

        private async Task<Result<string>> Export(string search)
        {
            return await _api.Products_ExportAsync(search);
        }

        private async Task<Result<string>> ExportSelected(int[] ids)
        {
            return await _api.Products_ExportByIdsAsync(ids.ToList());
        }


        private Task<bool> CreateOrEditProduct(TranslationDto productOrNull)
        {
            return Task.FromResult(false);
            //var parameters = new DialogParameters();
            //if (productOrNull != null)
            //{
            //    parameters.Add(nameof(AddEditProductModal.AddEditProductModel), productOrNull.MapTo<AddEditProductCommand>());
            //}

            //var options = new DialogOptionsEx { MaximizeButton = true, DragMode = MudDialogDragMode.Simple, CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };
            //var dialog = await _dialogService.ShowEx<AddEditProductModal>(productOrNull == null ? _localizer["Create"] : _localizer["Edit"], parameters, options);

            //var x = dialog.Dialog;
            //var result = await dialog.Result;
            //return !result.Cancelled;
        }
    }
}