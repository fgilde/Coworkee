using System;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Contracts.Enums;
using CleanArchitectureBase.Application.Features.Translations.Commands.AddEdit;
using CleanArchitectureBase.Application.Features.Translations.Export;
using CleanArchitectureBase.Client.Extensions;
using CleanArchitectureBase.Shared.Constants.Localization;
using CleanArchitectureBase.Shared.Extensions;
using CleanArchitectureBase.Shared.Wrapper;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.SignalR.Client;
using SDK;


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

        private List<TranslationDto> _translations;

        protected override async Task OnInitializedAsync()
        {
            HubConnection = await HubConnection.EnsureStartedAsync(_config.BackendOrigin);
        }

        private async Task<PaginatedResult<TranslationDto>> LoadPaged(int pageNumber, int pageSize, string _searchString, string[] orderings)
        {
            return await _api.Translations_GetAllPagedAsync(pageNumber, pageSize, _searchString, orderings);
        }

        private async Task<Result<List<TranslationDto>>> Load()
        {
            var cultureCode = CultureInfo.DefaultThreadCurrentCulture.AcceptHeaderCode();
            var local = clientLocalizationCache.ContainsKey(cultureCode)
                ? clientLocalizationCache[cultureCode]
                : Nextended.Core.Extensions.EnumerableExtensions.AddOrUpdate(clientLocalizationCache, cultureCode, _localizer.GetAllStrings(false).Select(s => new TranslationDto {CultureCode = cultureCode, Id = 0, Key = s.Name, Value = s.Value}).ToList())[cultureCode];

            if (!showCultureTranslations)
                local = local.Where(dto => !LocalizationConstants.ValidCultureName(dto.Key)).ToList();
           

            var server = await _api.Translations_GetAllAsync();

            _translations = server.Concat(local).DistinctBy(d => d.Key).ToList();
            return new Result<List<TranslationDto>>()
            {
                Succeeded = true,
                Data = _translations
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
            await _api.Translations_DeleteAsync(ids.ToList());
            AfterSend();
            return await Result.SuccessAsync() as Result;
        }

        private string GetName(TranslationDto arg)
        {
            return arg.Key;
        }

        private async Task Export(ExportServiceType exportServiceType, string search)
        {
            var query = new ExportTranslationsQuery { ExportServiceType = exportServiceType, SearchString = search, FilterByCurrentCulture = !advancedMode };
            if (!advancedMode)
                query.TranslationsToExport = _translations.ToArray();
            await (await _api.Translations_ExportAsync(query)).ForceDownloadAsync(_jsRuntime);
        }

        private async Task ExportSelected(ExportServiceType exportServiceType, int[] ids)
        {
            var query = new ExportTranslationsQuery {ExportServiceType = exportServiceType, Ids = ids, FilterByCurrentCulture = !advancedMode};
            if (!advancedMode)
                query.TranslationsToExport = _translations.ToArray();
            await (await _api.Translations_ExportAsync(query)).ForceDownloadAsync(_jsRuntime);
        }
        private Task ImportAsync(InputFileChangeEventArgs e)
        {
            return _api.Translations_ImportFileAsync(new FileParameter(e.File.OpenReadStream(), e.File.Name, e.File.ContentType));
        }

        private async Task<bool> SaveAll(TranslationDto[] arg)
        {
            foreach (var translationDto in arg.Where(dto => string.IsNullOrEmpty(dto.CultureCode)))
                translationDto.CultureCode = CultureInfo.DefaultThreadCurrentCulture.AcceptHeaderCode();
            
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

            return false;

        }

    }
}