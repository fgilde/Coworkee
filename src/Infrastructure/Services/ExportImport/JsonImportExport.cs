using System.Collections.Generic;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Contracts.Attributes;
using CleanArchitectureBase.Application.Contracts.Enums;
using CleanArchitectureBase.Application.Contracts.Services.ExportImport;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace CleanArchitectureBase.Infrastructure.Services.ExportImport;

[RegisterAs(typeof(IExportService), RegisterAsImplementation = true, ServiceLifetime = ServiceLifetime.Scoped)]
[RegisterAs(typeof(IImportService), RegisterAsImplementation = false, ServiceLifetime = ServiceLifetime.Scoped)]
public class JsonImportExport: IExportService, IImportService
{
    public ExportServiceType ExportService => ExportServiceType.Json;
    public IEnumerable<string> SupportedContentTypes => new[]
    {
        MediaTypeNames.Application.Json
    };


    public Task<byte[]> ExportAsync<TData>(IEnumerable<TData> data, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)), cancellationToken);
    }

    public Task<IEnumerable<TData>> ImportAsync<TData>(byte[] bytes, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<TData>>(Encoding.UTF8.GetString(bytes)), cancellationToken);

    }
}