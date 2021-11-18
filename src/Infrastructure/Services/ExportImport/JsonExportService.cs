using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Contracts.Enums;
using CleanArchitectureBase.Application.Contracts.Services.ExportImport;
using Newtonsoft.Json;

namespace CleanArchitectureBase.Infrastructure.Services.ExportImport;

public class JsonExportService: IExportService
{
    public ExportServiceType ExportService => ExportServiceType.Json;
    public Task<byte[]> ExportAsync<TData>(IEnumerable<TData> data, Dictionary<string, Func<TData, object>> mappers, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)), cancellationToken);
    }
}