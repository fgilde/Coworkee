using System.Threading.Tasks;
using lib.Coworkee.Client.JsInterop;
using Microsoft.JSInterop;
using SDK;

namespace lib.Coworkee.Client.Extensions;

public static class FileResponseExtensions
{
    public static Task ForceDownloadAsync(this FileResponse fileResponse, IJSRuntime js)
    {
        if (fileResponse?.IsSuccessStatusCode == true)
            return js.InvokeVoidAsync(JsNamespace.Get("BrowserHelper", "download"), fileResponse.JsDownloadObject).AsTask();
        return Task.CompletedTask;
    }
}