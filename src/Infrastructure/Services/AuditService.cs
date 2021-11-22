using CleanArchitectureBase.Infrastructure.Models.Audit;
using CleanArchitectureBase.Application.Responses.Audit;
using CleanArchitectureBase.Infrastructure.Contexts;
using CleanArchitectureBase.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Extensions;
using CleanArchitectureBase.Application.Contracts.Enums;
using CleanArchitectureBase.Application.Contracts.Services;
using CleanArchitectureBase.Application.Contracts.Services.ExportImport;
using CleanArchitectureBase.Infrastructure.Specifications;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Infrastructure.Services
{
    public class AuditService : IAuditService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExportService _excelService;
        private readonly IStringLocalizer<AuditService> _localizer;

        public AuditService(
            ApplicationDbContext context,
            IStringLocalizer<AuditService> localizer,
            IServiceProvider serviceProvider)
        {
            _context = context;
            _excelService = serviceProvider.GetServices<IExportService>().FirstOrDefault(s => s.ExportService == ExportServiceType.Excel); 
            _localizer = localizer;
        }

        public async Task<IResult<IEnumerable<AuditResponse>>> GetTrailsAsync(int limit = 1000, params string[] userIds)
        {
            var trails = await _context.AuditTrails.Where(a => !userIds.Any() || userIds.Contains(a.UserId)).OrderByDescending(a => a.Id).Take(limit).ToListAsync();
            var mappedLogs = trails.MapTo<List<AuditResponse>>();
            return await Result<IEnumerable<AuditResponse>>.SuccessAsync(mappedLogs);
        }

        public async Task<IResult<string>> ExportAsync(string[] userIds, string searchString = "", bool searchInOldValues = false, bool searchInNewValues = false)
        {
            var auditSpec = new AuditFilterSpecification(userIds, searchString, searchInOldValues, searchInNewValues);
            var trails = await _context.AuditTrails
                .Specify(auditSpec)
                .OrderByDescending(a => a.DateTime)
                .ToListAsync();
            var data = await _excelService.ExportAsync(trails,
                mappers: new Dictionary<string, Func<Audit, object>>
                {
                    { _localizer["Table Name"], item => item.TableName },
                    { _localizer["Type"], item => item.Type },
                    { _localizer["Date Time (Local)"], item => DateTime.SpecifyKind(item.DateTime, DateTimeKind.Utc).ToLocalTime().ToString("G", CultureInfo.CurrentCulture) },
                    { _localizer["Date Time (UTC)"], item => item.DateTime.ToString("G", CultureInfo.CurrentCulture) },
                    { _localizer["Primary Key"], item => item.PrimaryKey },
                    { _localizer["Old Values"], item => item.OldValues },
                    { _localizer["New Values"], item => item.NewValues },
                });

            return await Result<string>.SuccessAsync(data: Convert.ToBase64String(data));
        }
    }
}