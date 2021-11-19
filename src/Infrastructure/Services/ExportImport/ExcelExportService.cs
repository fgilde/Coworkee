using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Contracts.Enums;
using CleanArchitectureBase.Application.Contracts.Services.ExportImport;
using CsvHelper.Excel;
using Microsoft.Extensions.Localization;
using Nextended.Core.Extensions;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace CleanArchitectureBase.Infrastructure.Services.ExportImport
{
    public class ExcelExportService : IExportService
    {
        private readonly IStringLocalizer<ExcelExportService> _localizer;

        public ExportServiceType ExportService => ExportServiceType.Excel;

        public Task<byte[]> ExportAsync<TData>(
            IEnumerable<TData> data, Dictionary<string, 
            Func<TData, object>> mappers, 
            CancellationToken cancellationToken = default)
        {
            return ExportAsync(data, mappers, _localizer["Export_SheetName"], cancellationToken);
        }

        public ExcelExportService(IStringLocalizer<ExcelExportService> localizer)
        {
            _localizer = localizer;
        }

        public async Task<byte[]> ExportAsync<TData>(IEnumerable<TData> data, 
            Dictionary<string, Func<TData, object>> mappers,
            string sheetName,
            CancellationToken cancellationToken = default)
        {
            await using var stream = new MemoryStream();
            await using (var excelWriter = new ExcelWriter(stream, CultureInfo.CurrentCulture))
                await excelWriter.WriteRecordsAsync(data, cancellationToken);

            return stream.ToArray();

            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //using var p = new ExcelPackage();
            //p.Workbook.Properties.Author = "ApplicationMainIcon";
            //p.Workbook.Worksheets.Add(_localizer["Audit Trails"]);
            //var ws = p.Workbook.Worksheets[0];
            //ws.Name = sheetName;
            //ws.Cells.Style.Font.Size = 11;
            //ws.Cells.Style.Font.Name = "Calibri";

            //var colIndex = 1;
            //var rowIndex = 1;

            //var headers = mappers.Keys.Select(x => x).ToList();

            //foreach (var header in headers)
            //{
            //    var cell = ws.Cells[rowIndex, colIndex];

            //    var fill = cell.Style.Fill;
            //    fill.PatternType = ExcelFillStyle.Solid;
            //    fill.BackgroundColor.SetColor(Color.LightBlue);

            //    var border = cell.Style.Border;
            //    border.Bottom.Style =
            //        border.Top.Style =
            //            border.Left.Style =
            //                border.Right.Style = ExcelBorderStyle.Thin;

            //    cell.Value = header;

            //    colIndex++;
            //}

            //var dataList = data.ToList();
            //foreach (var item in dataList)
            //{
            //    colIndex = 1;
            //    rowIndex++;

            //    var result = headers.Select(header => mappers[header](item));

            //    foreach (var value in result)
            //    {
            //        ws.Cells[rowIndex, colIndex++].Value = value;
            //    }
            //}

            //using (ExcelRange autoFilterCells = ws.Cells[1, 1, dataList.Count + 1, headers.Count])
            //{
            //    autoFilterCells.AutoFilter = true;
            //    autoFilterCells.AutoFitColumns();
            //}

            //return await p.GetAsByteArrayAsync(cancellationToken);
        }
    }
}