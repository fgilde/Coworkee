using System;
using lib.Coworkee.Application.Common.Models;
using lib.Coworkee.Application.Common.Security;
using lib.Coworkee.Application.Contracts.Repositories;
using lib.Coworkee.Application.Features.Base.Import;
using lib.Coworkee.Application.Features.Translations.Commands.AddEdit;
using lib.Coworkee.Shared.Constants.Permission;
using Microsoft.Extensions.Localization;

namespace Coworkee.Application.Features.Translations.Import
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