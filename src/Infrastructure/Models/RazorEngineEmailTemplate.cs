using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Nextended.Core.Extensions;
using RazorEngineCore;
using lib.Coworkee.Application.Configurations;

namespace Coworkee.Infrastructure.Models;

public class RazorEngineEmailTemplate : RazorEngineTemplateBase
{
    private string clientUrl;
    private string serverUrl;

    public IStringLocalizer Localizer;
    public Func<string, object, string> IncludeCallback { get; set; }
    public Func<string> RenderBodyCallback { get; set; }
    public string Layout { get; set; }

    public void Initialize(IServiceProvider serviceProvider)
    {
        Localizer = serviceProvider.GetService<IStringLocalizer<RazorEngineEmailTemplate>>();
        var configuration = serviceProvider.GetService<ServerConfiguration>();
        var serverUrls = serviceProvider.GetService<IServer>().Features.Get<IServerAddressesFeature>().Addresses;

        clientUrl = configuration.ClientUrl;
        serverUrl = serverUrls.FirstOrDefault(u => u?.ToLower()?.StartsWith("https") == true) ?? serverUrls.FirstOrDefault();
    }

    public string Localize(string text, params object[] args)
    {
        bool hasArgs = args is { Length: > 0 };
        if (text is null)
            return null;
        return Localizer != null ? (hasArgs ? Localizer[text, args.Where(a => a != null).ToArray()] : Localizer[text]) : string.Format(text, args);
    }

    public string Table<T>(T context, string title, params string[] properties)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"<table class=\"detail-table\">");
        builder.AppendLine($"<tr><th colspan=\"2\">{title}</th></tr>");
        foreach (var property in properties)
        {
            var propertyValue = context.GetType().GetProperty(property)?.GetValue(context)?.ToString();
            builder.AppendLine($"<tr><td>{property}</td><td>{propertyValue}</td></tr>");
        }
        builder.AppendLine($"</table>");
        return builder.ToString();
    }

    public string Url(string pathAndQuery) => new UriBuilder(clientUrl).SetProperties(b => b.Path = pathAndQuery).Uri.AbsoluteUri;
    public string ServerUrl(string pathAndQuery) => new UriBuilder(serverUrl).SetProperties(b => b.Path = pathAndQuery).Uri.AbsoluteUri;

    public string Include(string key, object model = null) => IncludeCallback(key, model);

    public string RenderBody() => RenderBodyCallback();
}