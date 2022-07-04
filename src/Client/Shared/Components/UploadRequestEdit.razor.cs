using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeyRed.Mime;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Nextended.Core.Extensions;
using CleanArchitectureBase.Application.Contracts.Enums;
using CleanArchitectureBase.Application.Requests;
using CleanArchitectureBase.Client.JsInterop;
using CleanArchitectureBase.Shared.Helper;
using CleanArchitectureBase.Shared.Misc;
using MudBlazor.Extensions.Components;
using MudBlazor.Extensions.Extensions;
using Nextended.Blazor.Extensions;
using BrowserFileExtensions = Nextended.Blazor.Extensions.BrowserFileExtensions;

namespace CleanArchitectureBase.Client.Shared.Components;

public partial class UploadRequestEdit : IAsyncDisposable
{
    [Parameter] public string UploadFieldId { get; set; }
    [Parameter] public string[] MimeTypes { get; set; }
    [Parameter] public MimeTypeRestrictionType MimeRestrictionType { get; set; } = MimeTypeRestrictionType.WhiteList;
    [Parameter] public long? MaxFileSize { get; set; } = null;
    [Parameter] public int MaxHeight { get; set; }
    [Parameter] public int MinHeight { get; set; }

    [Parameter] public string Class { get; set; }
    [Parameter] public string Style { get; set; }

    [Parameter] public int MaxMultipleFiles { get; set; } = 100;
    [Parameter] public IList<UploadRequest> UploadRequests { get; set; }
    [Parameter] public bool AllowMultiple { get; set; } = true;
    [Parameter] public bool AllowFolderUpload { get; set; } = true;
    [Parameter] public bool AllowPreview { get; set; } = true;
    [Parameter] public bool ShowFileUploadButton { get; set; } = true;
    [Parameter] public bool ShowFolderUploadButton { get; set; } = true;
    [Parameter] public bool ShowClearButton { get; set; } = true;
    [Parameter] public bool AllowRemovingItems { get; set; } = true;
    [Parameter] public SelectItemsMode SelectItemsMode { get; set; } = SelectItemsMode.None;
    [Parameter] public bool AutoExtractZip { get; set; } = false;

    [Parameter] public UploadRequest UploadRequest
    {
        get => UploadRequests?.FirstOrDefault();
        set
        {
            var list = UploadRequests ??= new List<UploadRequest>();
            if (list.Count > 0)
                list[0] = value;
            else
                list.Add(value);
        }
    }
    [Parameter] public bool AllowDuplicates { get; set; }
    [Parameter] public bool DisplayErrors { get; set; } = true;
    [Parameter] public IList<UploadRequest> SelectedRequests { get; set; }
    [Parameter] public TimeSpan RemoveErrorAfter { get; set; } = TimeSpan.FromSeconds(5);
    [Parameter] public bool AutoRemoveError { get; set; } = true;
    [Parameter] public bool AllowDrop { get; set; } = true;

    [Parameter] public EventCallback<string> OnError { get; set; }
    [Parameter] public EventCallback<IList<UploadRequest>> UploadRequestsChanged { get; set; }
    [Parameter] public EventCallback<UploadRequest> UploadRequestRemoved { get; set; }
    [Parameter] public EventCallback<UploadRequest> UploadRequestChanged { get; set; }
    [Parameter] public EventCallback<IList<UploadRequest>> SelectedRequestsChanged { get; set; }

    private string _errorMessage = string.Empty;
    private CancellationTokenSource _tokenSource;
    private ElementReference dropZoneElement;
    private InputFile inputFile;
    private IJSObjectReference _module;
    private IJSObjectReference _dropZoneInstance;
    private List<UploadRequest> _withErrors = new();
    private string _accept;
    private string _acceptExtensions;

    protected override Task OnInitializedAsync()
    {
        UploadFieldId ??= $"{nameof(UploadRequestEdit)}-FileInput-{Guid.NewGuid()}";
        _accept = string.Join(",", (MimeTypes ?? Array.Empty<string>()).Distinct());
        var extensions = (MimeTypes?.Select(MimeTypesMap.GetExtension) ?? Array.Empty<string>()).ToList();
        if (MimeTypes?.Any(MimeTypeHelper.IsZip) == true)
            extensions.Add(".zip");
        _acceptExtensions = string.Join(",", extensions.Distinct());
        return base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && AllowDrop)
        {
            _module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/helper/dropZone.js");
            _dropZoneInstance = await _module.InvokeAsync<IJSObjectReference>("initializeFileDropZone", dropZoneElement, inputFile.Element, AllowFolderUpload);
        }
    }

    private async Task UploadFiles(InputFileChangeEventArgs e)
    {
        if (AllowMultiple)
            await Task.WhenAll(e.GetMultipleFiles(MaxMultipleFiles).Select(Add));
        else
        {
            (UploadRequests ??= new List<UploadRequest>()).Clear();
            await Add(e.File);
        }

        await RaiseChangedAsync();
    }

    private string StyleStr()
    {
        var str = $"{(MaxHeight != default ? $"max-height:{MaxHeight}px;" : string.Empty)} {(MinHeight != default ? $"min-height:{MinHeight}px;" : string.Empty)}";
        return $"{str}{Style}";
    }

    private Task Add(IEnumerable<IBrowserFile> files)
    {
        return Task.WhenAll(files.Select(Add));
    }

    private async Task Add(IBrowserFile file)
    {
        if (IsAllowed(file))
        {
            if (AutoExtractZip && file.IsZipFile())
            {
                await Add(await GetZipEntriesAsync(file));
                return;
            }

            var buffer = new byte[file.Size];
            var extension = Path.GetExtension(file.Name);
            await file.OpenReadStream(file.Size).ReadAsync(buffer);
            var existing = AllowDuplicates ? null : UploadRequests?.FirstOrDefault(r => r.Data.SequenceEqual(buffer));
            if (existing != null)
            {
                _withErrors.Add(existing);
                SetError(_localizer["The file ({0}) has already been added", file.Name]);
            }
            else
            {
                var request = new UploadRequest {Data = buffer, FileName = file.Name, UploadType = UploadType.Document, ContentType = file.ContentType, Extension = extension};
                (UploadRequests ??= new List<UploadRequest>()).Add(request);
            }
        }
    }

    private async Task<IList<ZipBrowserFile>> GetZipEntriesAsync(IBrowserFile file)
    {
        var stream = file.OpenReadStream(file.Size);
        await using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);

        return new ZipArchive(ms).Entries.Select(entry => new ZipBrowserFile(entry)).ToList();
    }

    private bool IsAllowed(IBrowserFile file)
    {
        if (MaxFileSize != null && MaxFileSize.Value != default && MaxFileSize.Value > 0 && file.Size > MaxFileSize)
            return !SetError(_localizer["The file has exceeded the maximum size of {0} with {1}. File size is {2}", BrowserFileExtensions.GetReadableFileSize(MaxFileSize.Value, _localizer), BrowserFileExtensions.GetReadableFileSize(file.Size - MaxFileSize.Value, _localizer), file.GetReadableFileSize(_localizer)]);

        if (!MimeTypeAllowed(file.ContentType))
            return !SetError(_localizer["Files of this type ({0}) are not allowed. Only following types are allowed '{1}'. Try one of these extensions ({2})", file.ContentType, _accept, _acceptExtensions]);
        
        if (UploadRequests?.Count >= Math.Max(1, MaxMultipleFiles))
            return !SetError(_localizer["A maximum of {0} files are allowed", MaxMultipleFiles]);
        
        return true;
    }

    private bool MimeTypeAllowed(string mimeType)
    {
        if (MimeTypes?.Any() != true) return true;
        var hasMatched = MimeTypeHelper.Matches(mimeType, MimeTypes);
        return (MimeRestrictionType != MimeTypeRestrictionType.WhiteList || hasMatched) && (MimeRestrictionType != MimeTypeRestrictionType.BlackList || !hasMatched);
    }

    private bool SetError(string message = default)
    {
        _tokenSource?.Cancel();
        _tokenSource = new CancellationTokenSource();
        var hasError = !string.IsNullOrWhiteSpace(message);
        _errorMessage = message;
        if (hasError)
        {
            if (AutoRemoveError)
                Task.Delay(RemoveErrorAfter).ContinueWith(_ => SetError(), _tokenSource.Token);
            OnError.InvokeAsync(_errorMessage);
        }
        else
        {
            _withErrors.Clear();
        }
        StateHasChanged();
        return hasError;
    }

    private Task RaiseChangedAsync()
    {
        return AllowMultiple
            ? UploadRequestsChanged.InvokeAsync(UploadRequests)
            : UploadRequestChanged.InvokeAsync(UploadRequest);
    } 


    public async ValueTask DisposeAsync()
    {
        if (_dropZoneInstance != null)
        {
            await _dropZoneInstance.InvokeVoidAsync("dispose");
            await _dropZoneInstance.DisposeAsync();
        }

        if (_module != null)
            await _module.DisposeAsync();
        
    }

    public void Remove(UploadRequest request)
    {
        UploadRequests.Remove(request);
        UploadRequestRemoved.InvokeAsync(request);
        StateHasChanged();
    }

    public void RemoveAll()
    {
        var array = UploadRequests?.ToArray() ?? Array.Empty<UploadRequest>();
        UploadRequests?.Clear();
        foreach (var item in array)
            UploadRequestRemoved.InvokeAsync(item);
        StateHasChanged();
    }

    public Task Upload(MouseEventArgs arg = null)
    {
        return _jsRuntime.InvokeVoidAsync(JsNamespace.Get("BrowserHelper", "clickOnElement"), "#" + UploadFieldId).AsTask();
    }

    public async Task UploadFolder(MouseEventArgs arg)
    {
        await _dropZoneInstance.InvokeVoidAsync("selectFolder");
    }

    public bool IsSelected(UploadRequest request)
    {
        return SelectedRequests?.Contains(request) == true;
    }

    private async Task Select(UploadRequest request, MouseEventArgs args)
    {
        if (SelectItemsMode != SelectItemsMode.None)
        {
            SelectedRequests ??= new List<UploadRequest>();

            if (SelectItemsMode == SelectItemsMode.Single || (SelectItemsMode == SelectItemsMode.MultiSelectWithCtrlKey && !args.CtrlKey))
                SelectedRequests.Clear();

            if (SelectedRequests.Contains(request))
                SelectedRequests.Remove(request);
            else
                SelectedRequests.Add(request);

            await SelectedRequestsChanged.InvokeAsync(SelectedRequests);
        }
    }

    private string GetIcon(UploadRequest request)
    {
        return BrowserFileExt.IconForFile(request.ContentType);
    }

    
    private async Task Preview(UploadRequest request)
    {
        //TODO SHow with IBrowserfile
        if (MimeTypeHelper.IsZip(request.ContentType))
        {
            var ms = new MemoryStream(request.Data);
            await MudExFileDisplayDialog.Show(_dialogService, ms, request.FileName, request.ContentType);
        }
        else
        {
            var dataUrl = await DataUrl.GetDataUrlAsync(request.Data, request.ContentType);
            await MudExFileDisplayDialog.Show(_dialogService, dataUrl, request.FileName, request.ContentType);
        }
    }


}
