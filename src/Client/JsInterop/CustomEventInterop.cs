using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace CleanArchitectureBase.Client.JsInterop;

public class CustomEventInterop<TEventArgs>: IDisposable
    //where TEventArgs : EventArgs 
{
    private readonly IJSRuntime _jsRuntime;
    private DotNetObjectReference<CustomEventHelper<TEventArgs>> _reference;
    
    public CustomEventInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public Task<CustomEventInterop<TEventArgs>> OnBlur(Func<TEventArgs, Task> callback, params string[] elementSelectors)
    {
        return OnBlur(elementSelectors, "click", callback);
    }

    public async Task<CustomEventInterop<TEventArgs>> OnBlur(string[] elementSelectors, string eventName, Func<TEventArgs, Task> callback)
    {
        await _jsRuntime.InvokeVoidAsync(
            JsNamespace.Get("EventHelper", "addCustomEventListenerWhenNotIn"), elementSelectors, eventName,
            _reference = DotNetObjectReference.Create(new CustomEventHelper<TEventArgs>(callback))
        );
        return this;
    }

    public async Task<CustomEventInterop<TEventArgs>> AddEventListener(string eventName, Func<TEventArgs, Task> callback)
    {
        await _jsRuntime.InvokeVoidAsync(
            JsNamespace.Get("EventHelper", "addCustomEventListener"), eventName,
            _reference = DotNetObjectReference.Create(new CustomEventHelper<TEventArgs>(callback))
        );
        return this;
    }

    public void Dispose()
    {
        _reference?.Dispose();
    }
}

public class CustomEventHelper<TEventArgs> 
    //where TEventArgs: EventArgs
{
    private readonly Func<TEventArgs, Task> _callback;

    public CustomEventHelper(Func<TEventArgs, Task> callback)
    {
        _callback = callback;
    }

    [JSInvokable]
    public Task OnCustomEvent(TEventArgs args)
    {
        return _callback(args);
    }
}