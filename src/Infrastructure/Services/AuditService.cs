using CleanArchitectureBase.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Extensions;
using CleanArchitectureBase.Application.Common.Models;
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
        private readonly IStringLocalizer<AuditService> _localizer;
        private readonly IServiceProvider _serviceProvider;

        public AuditService(
            ApplicationDbContext context,
            IStringLocalizer<AuditService> localizer,
            IServiceProvider serviceProvider)
        {
            _context = context;
            _localizer = localizer;
            _serviceProvider = serviceProvider;
        }

        public async Task<IReadOnlyCollection<AuditDto>> GetTrailsAsync(int limit = 1000, params string[] userIds)
        {
            var trails = await _context.AuditTrails.Where(a => !userIds.Any() || userIds.Contains(a.UserId)).OrderByDescending(a => a.Id).Take(limit).ToListAsync();
            return trails.MapTo<List<AuditDto>>().AsReadOnly();
        }

        public async Task<byte[]> ExportAsync(
            ExportServiceType exportServiceType, 
            string[] userIds, 
            string searchString = "", 
            bool searchInOldValues = false, 
            bool searchInNewValues = false, 
            CancellationToken cancellationToken = default)
        {
            var exportService = _serviceProvider.GetServices<IExportService>().First(s => s.ExportService == exportServiceType);
            var auditSpec = new AuditFilterSpecification(userIds, searchString, searchInOldValues, searchInNewValues);
            var trails = await _context.AuditTrails
                .Specify(auditSpec)
                .OrderByDescending(a => a.DateTime)
                .ToListAsync(cancellationToken: cancellationToken);
            return await exportService.ExportAsync(trails, cancellationToken);
        }
    }
}