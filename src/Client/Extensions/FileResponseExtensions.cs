using System.Threading.Tasks;
using CleanArchitectureBase.Client.JsInterop;
using Microsoft.JSInterop;
using SDK;

namespace CleanArchitectureBase.Client.Extensions;

public static class FileResponseExtensions
{
    public static Task ForceDownloadAsync(this FileResponse fileResponse, IJSRuntime js)
    {
        if (fileResponse?.IsSuccessStatusCode == true)
            return js.InvokeVoidAsync(JsNamespace.Get("BrowserHelper", "download"), fileResponse.JsDownloadObject).AsTask();
        return Task.CompletedTask;
    }
}