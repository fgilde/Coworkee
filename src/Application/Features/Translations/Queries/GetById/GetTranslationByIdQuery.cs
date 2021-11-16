using System;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Common.Security;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Features.Base.Queries;
using CleanArchitectureBase.Domain.Entities.Localization;
using CleanArchitectureBase.Shared.Constants.Permission;

namespace CleanArchitectureBase.Application.Features.Translations.Queries.GetById
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