using System;
using System.Collections.Generic;
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
public class JsonExportService: IExportService
{
    public ExportServiceType ExportService => ExportServiceType.Json;
    public Task<byte[]> ExportAsync<TData>(IEnumerable<TData> data, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)), cancellationToken);
    }
}