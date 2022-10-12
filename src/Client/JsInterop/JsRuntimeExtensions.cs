using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace CleanArchitectureBase.Client.JsInterop;

public static class JsRuntimeExtensions
{
    public static ValueTask ObserveMudTabsForStickMerge(this IJSRuntime jsRuntime, string selectorObserveElement)
    {
        return jsRuntime.InvokeVoidAsync("eval", "new IntersectionObserver(([e]) => { console.log('STICK STUCK CHANGE'); document.querySelector('.mud-tabs-stick-merge .mud-tabs-toolbar').toggleAttribute('stuck', e.intersectionRatio < 1);}, {threshold: [1]}).observe(document.querySelector('"+ selectorObserveElement + "'));");
    }
}