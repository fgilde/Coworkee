using Coworkee.Client.Blazor.Components;
using Coworkee.Client.Blazor.Security;
using Coworkee.Client.Blazor.Localization;
using Coworkee.Client.Blazor.Api;
using Coworkee.Client.Blazor.Components.Data;
using Coworkee.Client.Blazor.Components.Files;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MyApp.Contracts.Documents;
using MyApp.Web.Client.Api;
using MyApp.Web.Client.Components;

namespace MyApp.Web.Client.Pages.Documents;

public partial class DocumentStore
{
    [Inject] private CoworkeeLocalizer L { get; set; } = null!;

    private static readonly string[] SearchFields = [nameof(DocumentDto.Title), nameof(DocumentDto.Description), nameof(DocumentDto.FileName)];
    private CoworkeeDataTable<DocumentDto> _table = null!;

    [Inject] private IDocumentsApi Api { get; set; } = null!;

    [Inject] private IDialogService Dialogs { get; set; } = null!;

    [Inject] private PermissionStore Permissions { get; set; } = null!;

    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    [CascadingParameter] private Task<Microsoft.AspNetCore.Components.Authorization.AuthenticationState> AuthenticationState { get; set; } = null!;

    /// <summary>Only the documents the current user uploaded, as the account page shows them.</summary>
    [Parameter] public bool Mine { get; set; }

    private string? _filter;
    private bool _ready;

    protected override async Task OnInitializedAsync()
    {
        if (Mine && (await AuthenticationState).User.FindFirst("sub")?.Value is { } me)
        {
            _filter = $"OwnerId eq {me}";
        }

        _ready = true;
    }

    internal static string FormatSize(long bytes) => bytes switch
    {
        < 1024 => $"{bytes} B",
        < 1024 * 1024 => $"{bytes / 1024d:0.#} KB",
        _ => $"{bytes / (1024d * 1024):0.#} MB",
    };

    private Task CreateAsync() => DocumentDialog.ShowAsync(Dialogs, null);

    private Task EditAsync(DocumentDto document) => DocumentDialog.ShowAsync(Dialogs, document);

    private async Task AttributesAsync(DocumentDto document) =>
        await ExtendedAttributesDialog.ShowAsync(Dialogs, L["Attributes"], "Documents", document.Id, readOnly: !await Permissions.HasAsync(DocumentPermissions.Documents.Edit));

    private Task PreviewAsync(DocumentDto document) => FilePreviewDialog.ShowAsync(Dialogs, DocumentUrls.Content(document.Id), document.MimeType, document.FileName);

    private Task DeleteAsync(IReadOnlyCollection<DocumentDto> documents) => Snackbar.RunAsync(() => Api.DeleteDocumentsAsync([.. documents.Select(d => d.Id)]));
}
