using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Nextended.Core;

namespace CleanArchitectureBase.Server.Extensions;

public static class HttpRequestExtensions
{
    private const string RequestedWithHeader = "X-Requested-With";
    private const string XmlHttpRequest = "XMLHttpRequest";

    public static bool IsAjaxRequest(this HttpRequest request)
    {
        return request.ThrowIfNull(nameof(request)).Headers[RequestedWithHeader] == XmlHttpRequest || request.HasReferer();
    }

    public static IEnumerable<Uri> GetRefererUris(this HttpRequest request)
    {
        return request?.Headers.Referer.Where(s => !string.IsNullOrEmpty(s)).Select(s => new Uri(s, UriKind.RelativeOrAbsolute));
    }

    public static bool HasReferer(this HttpRequest request)
    {
        return request?.Headers.Referer.Count > 0;
    }
}