using System;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Features.Base.Queries;
using CleanArchitectureBase.Domain.Entities.Localization;

namespace CleanArchitectureBase.Application.Features.Translations.Queries.GetById
{
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