using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace lib.Coworkee.Client.JsInterop;

public static class JsRuntimeExtensions
{
    public static ValueTask ObserveMudTabsForStickMerge(this IJSRuntime jsRuntime, string selectorObserveElement)
    {
        return jsRuntime.InvokeVoidAsync("eval", "new IntersectionObserver(([e]) => { try { document.querySelector('.mud-tabs-stick-merge .mud-tabs-tabbar')?.toggleAttribute('stuck', e.intersectionRatio < 1); }catch(e){} }, {threshold: [1]}).observe(document.querySelector('" + selectorObserveElement + "'));");
    }
}