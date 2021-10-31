using System;
using CleanArchitectureBase.Application.Dtos;
using CleanArchitectureBase.Application.Features.Base.Queries;
using CleanArchitectureBase.Application.Interfaces.Repositories;
using CleanArchitectureBase.Domain.Entities.Localization;

namespace CleanArchitectureBase.Application.Features.Translations.Queries.GetById
{
    public class GetTranslationByIdQuery: GetByIdQueryBase<int, TranslationDto>
    {
        public GetTranslationByIdQuery(int id) : base(id)
        {}
    }

    internal class GetTranslationQueryHandler : GetByIdQueryHandlerBase<GetTranslationByIdQuery, int, TranslationDto, Translation>
    {
        public GetTranslationQueryHandler(IUnitOfWork<int> unitOfWork, IServiceProvider provider) : base(unitOfWork, provider)
        {}
    }
}