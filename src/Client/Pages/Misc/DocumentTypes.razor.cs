using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Contracts.Enums;
using Coworkee.Client.Extensions;
using Coworkee.Shared.Wrapper;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Coworkee.Client.Pages.Misc
{
    public partial class DocumentTypes
    {

        [Parameter]
        public string Action { get; set; }

        [Parameter]
        public string Id { get; set; }


        private async Task<Result<List<DocumentTypeDto>>> Load(CancellationToken cancellationToken)
        {
            return new Result<List<DocumentTypeDto>>
            {
                Succeeded = true,
                Data = (await _api.DocumentTypes_GetAllAsync(cancellationToken)).ToList()
            };
        }

        private Task<DocumentTypeDto> FindById(int id, IEnumerable<DocumentTypeDto> loaded)
        {
            return Task.FromResult(loaded.FirstOrDefault(p => p.Id == id));
        }

        private int GetId(DocumentTypeDto documentType)
        {
            return documentType.Id;
        }

        private async Task<Result> Delete(int[] ids)
        {
            await _api.DocumentTypes_DeleteAsync(ids.ToList());
            return new Result { Succeeded = true };
        }

        private string GetName(DocumentTypeDto arg)
        {
            return arg.Name;
        }

        private async Task Export(ExportServiceType exportServiceType, string search)
        {
            await (await _api.DocumentTypes_ExportAsync(exportServiceType: exportServiceType, searchString: search)).ForceDownloadAsync(_jsRuntime);
        }

        private async Task ExportSelected(ExportServiceType exportServiceType, int[] ids)
        {
            await (await _api.DocumentTypes_ExportAsync(exportServiceType: exportServiceType, ids: ids)).ForceDownloadAsync(_jsRuntime);
        }

        private async Task<bool> CreateOrEdit(DocumentTypeDto arg)
        {
            var parameters = new DialogParameters();
            if (arg != null)
            {
                parameters.Add(nameof(AddEditDocumentTypeModal.AddEditDocumentTypeModel), arg);
            }
            
            var dialog = await _dialogService.ShowWithDefaultOptionsAsync<AddEditDocumentTypeModal>(arg == null ? _localizer["Create"] : _localizer["Edit"], parameters);
            var result = await dialog.Result;

            return !result.Cancelled;
        }
    }
}
