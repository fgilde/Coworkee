using Coworkee.Client.Blazor.Api;
using Coworkee.Client.Blazor.Components.Data;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MyApp.Contracts.Documents;
using MyApp.Web.Client.Api;
using MyApp.Web.Client.Components;

namespace MyApp.Web.Client.Pages.Documents;

public partial class DocumentStore
{
    private static readonly string[] SearchFields = [nameof(DocumentDto.Title), nameof(DocumentDto.Description), nameof(DocumentDto.FileName)];
    private CoworkeeDataTable<DocumentDto> _table = null!;
    private DocumentDto? _preview;

    [Inject] private IDocumentsApi Api { get; set; } = null!;

    [Inject] private IDialogService Dialogs { get; set; } = null!;

    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    internal static string FormatSize(long bytes) => bytes switch
    {
        < 1024 => $"{bytes} B",
        < 1024 * 1024 => $"{bytes / 1024d:0.#} KB",
        _ => $"{bytes / (1024d * 1024):0.#} MB",
    };

    private Task CreateAsync() => DocumentDialog.ShowAsync(Dialogs, null);

    private Task EditAsync(DocumentDto document) => DocumentDialog.ShowAsync(Dialogs, document);

    private async Task DeleteAsync(IReadOnlyCollection<DocumentDto> documents)
    {
        if (await Snackbar.RunAsync(() => Api.DeleteDocumentsAsync([.. documents.Select(d => d.Id)])) && documents.Any(d => d.Id == _preview?.Id))
        {
            _preview = null;
        }
    }
}
