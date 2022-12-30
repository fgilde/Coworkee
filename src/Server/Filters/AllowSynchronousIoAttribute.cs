using System;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Coworkee.Server.Filters;


/// <summary>
/// Use this attribute to enable Synchronous IO on a per-request basis:
/// 
/// https://github.com/aspnet/AspNetCore/issues/7644
/// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel?view=aspnetcore-3.0#synchronous-io
///
/// AllowSynchronousIO enables or disables synchronous IO APIs, such as HttpRequest.Body.Read, HttpResponse.Body.Write, and
/// Stream.Flush.
/// These APIs are a source of thread starvation leading to app crashes. In 3.0, AllowSynchronousIO is disabled by default.
/// For more information, see the Synchronous IO section in the Kestrel article.
/// If synchronous IO is needed, it can be enabled by configuring the AllowSynchronousIO option on the server being used
/// (when calling ConfigureKestrel, for example, if using Kestrel). Note that servers (Kestrel, HttpSys, TestServer, etc.) all have
/// their own AllowSynchronousIO option that won't affect other servers. Synchronous IO can be enabled for all servers on a per-request
/// basis using the IHttpBodyControlFeature.AllowSynchronousIO option
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AllowSynchronousIoAttribute : Attribute, IResourceFilter
{
    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        var syncIoFeature = context.HttpContext.Features.Get<IHttpBodyControlFeature>();
        if (syncIoFeature != null)
        {
            syncIoFeature.AllowSynchronousIO = true;
        }
    }

    public void OnResourceExecuted(ResourceExecutedContext context)
    {
    }
}