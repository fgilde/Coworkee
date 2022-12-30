using System;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Coworkee.Client.Extensions;

public static class ServiceAccessor
{
    public static IServiceProvider ServiceProvider { get; private set; }

    public static T Get<T> ()=> ServiceProvider.GetService<T> ();

    public static WebAssemblyHost MakeStaticAccessible(this WebAssemblyHost host)
    {
        ServiceProvider = host.Services;
        return host;
    }
}