using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using lib.Coworkee.Application.Contracts.Enums;

namespace lib.Coworkee.Application.Contracts.Services.ExportImport;

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