using System;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Common.Security;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Features.Base.Import;
using CleanArchitectureBase.Application.Features.Translations.Commands.AddEdit;
using CleanArchitectureBase.Shared.Constants.Permission;
using Microsoft.Extensions.Localization;

namespace CleanArchitectureBase.Application.Features.Translations.Import
{
    [CustomAuthorize(Policies = new[] {Permissions.Translations.Export})]
    public class ImportTranslationsQuery : ImportQueryBase<TranslationDto>
    {}

    internal class ImportTranslationsQueryHandler : ImportQueryHandlerBase<ImportTranslationsQuery, int, TranslationDto, AddEditTranslationsCommand>
    {

        public ImportTranslationsQueryHandler(IUnitOfWork<int> unitOfWork, IStringLocalizer<ImportTranslationsQueryHandler> localizer, IServiceProvider serviceProvider)
            : base(unitOfWork, localizer, serviceProvider)
        { }
    }
}