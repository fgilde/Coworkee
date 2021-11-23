using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Contracts.Attributes;
using CleanArchitectureBase.Application.Contracts.Enums;
using CleanArchitectureBase.Application.Contracts.Services.ExportImport;
using CsvHelper.Excel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;


namespace CleanArchitectureBase.Infrastructure.Services.ExportImport
{
    [RegisterAs(typeof(IExportService), RegisterAsImplementation = true, ServiceLifetime = ServiceLifetime.Scoped)]
    public class ExcelExportService : IExportService
    {
        private readonly IStringLocalizer<ExcelExportService> _localizer;

        public ExportServiceType ExportService => ExportServiceType.Excel;

        public ExcelExportService(IStringLocalizer<ExcelExportService> localizer)
        {
            _localizer = localizer;
        }

        public async Task<byte[]> ExportAsync<TData>(IEnumerable<TData> data,
            CancellationToken cancellationToken = default)
        {
            await using var stream = new MemoryStream();
            await using (var excelWriter = new ExcelWriter(stream, CultureInfo.CurrentCulture))
                await excelWriter.WriteRecordsAsync(data, cancellationToken);

            return stream.ToArray();
        }
    }
}