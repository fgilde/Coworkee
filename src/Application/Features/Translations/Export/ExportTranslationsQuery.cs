using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Common.Security;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Features.Base.Export;
using CleanArchitectureBase.Application.Specifications.Base;
using CleanArchitectureBase.Application.Specifications.Translations;
using CleanArchitectureBase.Domain.Entities.Localization;
using CleanArchitectureBase.Shared.Constants.Permission;
using Microsoft.Extensions.Localization;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Application.Features.Translations.Export
{
    [CustomAuthorize(Policies = new[] {Permissions.Translations.Export})]
    public class ExportTranslationsQuery : ExportQueryBase<int>
    {
        public TranslationDto[] TranslationsToExport { get; set; }
        public bool FilterByCurrentCulture { get; set; } = true;
    }

    internal class ExportTranslationsQueryHandler : ExportQueryHandlerBase<ExportTranslationsQuery, int, Translation>
    {

        public ExportTranslationsQueryHandler(IUnitOfWork<int> unitOfWork, IStringLocalizer<ExportTranslationsQueryHandler> localizer, IServiceProvider serviceProvider)
            : base(unitOfWork, localizer, serviceProvider)
        { }

        protected override ISpecification<Translation> GetFilterSpecification(ExportTranslationsQuery query)
        {
            return new TranslationFilterSpecification(query.SearchString, query.FilterByCurrentCulture ? Thread.CurrentThread.CurrentCulture.Name : null);
        }

        public override async Task<byte[]> Handle(ExportTranslationsQuery request, CancellationToken cancellationToken)
        {
            if (request.TranslationsToExport?.Any() == true)
            {
                return await GetExportService(request).ExportAsync(
                    request.TranslationsToExport
                        .Where(p => string.IsNullOrEmpty(request.SearchString) || p.Key.Contains(request.SearchString, StringComparison.CurrentCultureIgnoreCase) || p.Value.Contains(request.SearchString,StringComparison.InvariantCultureIgnoreCase))
                        .Where(p => !request.FilterByCurrentCulture || p.CultureCode == Thread.CurrentThread.CurrentCulture.Name)
                        .MapElementsTo<Translation>(), cancellationToken);
            }

            return await base.Handle(request, cancellationToken);
        }
    }
}