using System;
using lib.Coworkee.Application.Common.Models;
using lib.Coworkee.Application.Common.Security;
using lib.Coworkee.Application.Contracts.Repositories;
using lib.Coworkee.Application.Features.Base.Queries;
using lib.Coworkee.Domain.Entities.Localization;
using lib.Coworkee.Shared.Constants.Permission;

namespace Coworkee.Application.Features.Translations.Queries.GetById
{
    [CustomAuthorize(Policies = new[] { Permissions.Translations.View })]
    public class GetTranslationByIdQuery: GetByIdQueryBase<int, TranslationDto>
    {
        public GetTranslationByIdQuery(int id) : base(id)
        {}
    }

    internal class GetTranslationByIdQueryHandler : GetByIdQueryHandlerBase<GetTranslationByIdQuery, int, TranslationDto, Translation>
    {
        public GetTranslationByIdQueryHandler(IUnitOfWork<int> unitOfWork, IServiceProvider provider) : base(unitOfWork, provider)
        {}
    }
}