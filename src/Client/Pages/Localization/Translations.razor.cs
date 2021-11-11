using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Dtos;
using CleanArchitectureBase.Application.Features.Translations.Commands.AddEdit;
using CleanArchitectureBase.Client.Extensions;
using CleanArchitectureBase.Shared.Constants.Localization;
using CleanArchitectureBase.Shared.Wrapper;
using Microsoft.AspNetCore.SignalR.Client;


namespace CleanArchitectureBase.Client.Pages.Localization
{
    public partial class Translations
    {
        private Dictionary<string, List<TranslationDto>> clientLocalizationCache = new();

        [Parameter]
        public string Action { get; set; }

        [Parameter]
        public string Id { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }

        protected override async Task OnInitializedAsync()
        {
            HubConnection = await HubConnection.EnsureStartedAsync(_navigationManager);
        }

        private async Task<PaginatedResult<TranslationDto>> LoadPaged(int pageNumber, int pageSize, string _searchString, string[] orderings)
        {
            return await _api.Translations_GetAllPagedAsync(pageNumber, pageSize, _searchString, orderings);
        }

        private async Task<Result<List<TranslationDto>>> Load()
        {
            var cultureCode = CultureInfo.DefaultThreadCurrentCulture?.TwoLetterISOLanguageName;
            var local = clientLocalizationCache.ContainsKey(cultureCode)
                ? clientLocalizationCache[cultureCode]
                : Nextended.Core.Extensions.EnumerableExtensions.AddOrUpdate(clientLocalizationCache, cultureCode, _localizer.GetAllStrings(false).Select(s => new TranslationDto {CultureCode = cultureCode, Id = 0, Key = s.Name, Value = s.Value}).ToList())[cultureCode];

            if (!showCultureTranslations)
                local = local.Where(dto => !LocalizationConstants.ValidCultureName(dto.Key)).ToList();
           

            var server = await _api.Translations_GetAllAsync();

            var res = server.Concat(local).DistinctBy(d => d.Key);
            return new Result<List<TranslationDto>>()
            {
                Succeeded = true,
                Data = res.ToList()
            };
        }

        private async Task<TranslationDto> FindById(int id, IEnumerable<TranslationDto> loaded)
        {
            var res = loaded.FirstOrDefault(p => p.Id == id) ?? (await _api.Translations_GetByIdAsync(id))?.Data;
            return res;
        }

        private int GetId(TranslationDto translation)
        {
            return translation?.Id ?? default(int);
        }

        private async Task<Result> DeleteTranslations(int[] ids)
        {
            var res = await _api.Translations_DeleteAsync(ids.ToList());
            AfterSend();
            return res;
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

        private async Task<bool> SaveAll(TranslationDto[] arg)
        {
            await _api.Translations_PostAsync(new AddEditTranslationsCommand { Items = arg });
            AfterSend();
            return true;
        }

        private void AfterSend()
        {
            clientLocalizationCache.Clear();
        }

        private async Task<bool> CreateOrEdit(TranslationDto productOrNull)
        {
            if (productOrNull != null)
            {
                await _api.Translations_PostAsync(new AddEditTranslationsCommand { Items = new[] { productOrNull } });
                AfterSend();
                return true;
            }
            else
            {

                return false;
            }
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