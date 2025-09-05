using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using lib.Coworkee.Application.Contracts.Enums;
using lib.Coworkee.Application.Contracts.Services.ExportImport;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Excel;
using Microsoft.Extensions.DependencyInjection;
using Nextended.Core;
using Nextended.Core.Attributes;


namespace lib.Coworkee.Infrastructure.Services.ExportImport
{
    [RegisterAs(typeof(IExportService), RegisterAsImplementation = true, ServiceLifetime = ServiceLifetime.Scoped)]
    [RegisterAs(typeof(IImportService), RegisterAsImplementation = false, ServiceLifetime = ServiceLifetime.Scoped)]
    public class ExcelImportExport : IExportService, IImportService
    {
        public ExportServiceType ExportService => ExportServiceType.Excel;

        public IEnumerable<string> SupportedContentTypes => [ MimeType.OpenXml, MimeType.Xls ];

        public async Task<byte[]> ExportAsync<TData>(IEnumerable<TData> data,
            CancellationToken cancellationToken = default)
        {
            await using var stream = new MemoryStream();
            await using (var excelWriter = new ExcelWriter(stream, CultureInfo.CurrentCulture))
                await excelWriter.WriteRecordsAsync(data, cancellationToken);

            return stream.ToArray();
        }

        public Task<IEnumerable<TData>> ImportAsync<TData>(byte[] bytes, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                using var stream = new MemoryStream(bytes);
                CsvConfiguration config = new CsvConfiguration(CultureInfo.CurrentUICulture)
                {
                    BadDataFound = null,
                    DetectColumnCountChanges = false,
                    MissingFieldFound = null,
                };
                using var parser = new ExcelParser(stream, string.Empty, config);
                using var reader = new CsvReader(parser);
                return reader.GetRecords<TData>().ToList().AsEnumerable();
            }, cancellationToken);

        }
    }
}