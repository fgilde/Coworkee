using System.Collections.Generic;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using lib.Coworkee.Application.Contracts.Enums;
using lib.Coworkee.Application.Contracts.Services.ExportImport;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Nextended.Core.Attributes;

namespace lib.Coworkee.Infrastructure.Services.ExportImport;

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