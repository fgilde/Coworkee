using System;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Common.Security;
using Coworkee.Application.Contracts.Repositories;
using Coworkee.Application.Features.Base.Queries;
using Coworkee.Domain.Entities.Localization;
using Coworkee.Shared.Constants.Permission;

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