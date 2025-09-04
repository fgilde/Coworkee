using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Common.Security;
using Coworkee.Application.Contracts.Repositories;
using Coworkee.Application.Features.Base.Export;
using Coworkee.Application.Specifications.Base;
using Coworkee.Application.Specifications.Translations;
using Coworkee.Domain.Entities.Localization;
using Coworkee.Shared.Constants.Permission;
using Microsoft.Extensions.Localization;

namespace Coworkee.Application.Features.Translations.Export
{
    [CustomAuthorize(Policies = new[] {Permissions.Translations.Export})]
    public class ExportTranslationsQuery : ExportQueryBase<int>
    {
        public TranslationDto[] TranslationsToExport { get; set; }
        public bool FilterByCurrentCulture { get; set; } = true;
    }

    internal class ExportTranslationsQueryHandler : ExportQueryHandlerBase<ExportTranslationsQuery, int, TranslationDto, Translation>
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
                        , cancellationToken);
            }

            return await base.Handle(request, cancellationToken);
        }
    }
}