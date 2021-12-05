using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Contracts.Enums;

namespace CleanArchitectureBase.Application.Contracts.Services.ExportImport;

public interface IExportService
{
    public ExportServiceType ExportService { get; }

    Task<byte[]> ExportAsync<TData>(IEnumerable<TData> data, CancellationToken cancellationToken = default);
}

public interface IImportService
{
    public IEnumerable<string> SupportedContentTypes { get; }

    Task<IEnumerable<TData>> ImportAsync<TData>(byte[] bytes, CancellationToken cancellationToken = default);
}