using Coworkee.Client.Blazor.Components.Data;
using Coworkee.Client.Blazor.Localization;
using Coworkee.Client.Blazor.Api;
using Coworkee.Client.Blazor.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using MyApp.Contracts.Documents;
using MyApp.Web.Client.Api;

namespace MyApp.Web.Client.Components;

public partial class DocumentDialog
{
    [Inject] private CoworkeeLocalizer L { get; set; } = null!;

    private const long MaxFileBytes = 100L * 1024 * 1024;
    private MudForm _form = null!;
    private IReadOnlyList<DocumentTypeDto> _types = [];
    private IBrowserFile? _file;
    private bool _saving;

    [CascadingParameter] private IMudDialogInstance Dialog { get; set; } = null!;

    [Inject] private IODataClient OData { get; set; } = null!;

    [Inject] private IDocumentsApi Api { get; set; } = null!;

    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    [Inject] private NavigationManager Nav { get; set; } = null!;

    [Parameter] public string Title { get; set; } = string.Empty;

    [Parameter] public Guid? Id { get; set; }

    [Parameter] public DocumentDto? Document { get; set; }

    [Parameter] public UpdateDocumentRequest Model { get; set; } = new();

    public static async Task<bool> ShowAsync(IDialogService dialogs, DocumentDto? document)
    {
        var parameters = new DialogParameters<DocumentDialog>
        {
            { d => d.Title, document is null ? "Upload a document" : "Edit document" },
            { d => d.Id, document?.Id },
            { d => d.Document, document },
            {
                d => d.Model, new UpdateDocumentRequest
                {
                    Title = document?.Title ?? string.Empty,
                    Description = document?.Description,
                    IsPublic = document?.IsPublic ?? false,
                    DocumentTypeId = document?.DocumentTypeId,
                }
            },
        };
        var dialog = await dialogs.ShowSideSheetAsync<DocumentDialog>(string.Empty, parameters);
        return await dialog.Result is { Canceled: false };
    }

    protected override async Task OnInitializedAsync() =>
        _types = (await OData.QueryAsync<DocumentTypeDto>("DocumentTypes", new ODataQuery { OrderBy = "Name", Top = 1000, Count = false })).Items;

    private async Task SaveAsync()
    {
        await _form.ValidateAsync();
        if (!_form.IsValid)
        {
            return;
        }

        if (Id is null && _file is null)
        {
            Snackbar.Add(L["Choose a file to upload."], Severity.Warning);
            return;
        }

        _saving = true;
        if (await Snackbar.RunAsync(SendAsync, Id is null ? L["Document uploaded"] : L["Document saved"]))
        {
            Dialog.Close(DialogResult.Ok(true));
        }

        _saving = false;
    }

    private async Task SendAsync()
    {
        if (Id is { } id)
        {
            await Api.UpdateDocumentAsync(id, Model);
            return;
        }

        await using var content = _file!.OpenReadStream(MaxFileBytes);
        await Api.UploadDocumentAsync(Model, _file.Name, content);
    }

    private void Cancel() => Dialog.Cancel();
}
