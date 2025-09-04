using System;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Common.Security;
using Coworkee.Application.Contracts.Repositories;
using Coworkee.Application.Features.Base.Import;
using Coworkee.Application.Features.Translations.Commands.AddEdit;
using Coworkee.Shared.Constants.Permission;
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