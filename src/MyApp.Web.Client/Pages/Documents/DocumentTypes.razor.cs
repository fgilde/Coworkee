using Coworkee.Client.Blazor.Localization;
using Coworkee.Client.Blazor.Api;
using Coworkee.Client.Blazor.Components.Data;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MyApp.Contracts.Documents;
using MyApp.Web.Client.Api;

namespace MyApp.Web.Client.Pages.Documents;

public partial class DocumentTypes
{
    [Inject] private CoworkeeLocalizer L { get; set; } = null!;

    private static readonly string[] SearchFields = [nameof(DocumentTypeDto.Name), nameof(DocumentTypeDto.Description)];
    private CoworkeeDataTable<DocumentTypeDto> _table = null!;

    [Inject] private IDocumentsApi Api { get; set; } = null!;

    [Inject] private IDialogService Dialogs { get; set; } = null!;

    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    private Task CreateAsync() => EditAsync(null, new AddEditDocumentTypeRequest());

    private Task EditAsync(DocumentTypeDto type) => EditAsync(type.Id, new AddEditDocumentTypeRequest { Name = type.Name, Description = type.Description });

    private async Task EditAsync(Guid? id, AddEditDocumentTypeRequest model)
    {
        if (await Dialogs.ShowEditAsync(L[id is null ? "New document type" : "Edit document type"], model, saved => Api.SaveDocumentTypeAsync(id, saved)))
        {
            Snackbar.Add(L["Document type saved"], Severity.Success);
        }
    }

    private Task DeleteAsync(IReadOnlyCollection<DocumentTypeDto> types) => Snackbar.RunAsync(() => Api.DeleteDocumentTypesAsync([.. types.Select(t => t.Id)]));
}
