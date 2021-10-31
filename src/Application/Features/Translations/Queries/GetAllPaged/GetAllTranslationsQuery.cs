using System;
using CleanArchitectureBase.Application.Interfaces.Repositories;
using MediatR;
using CleanArchitectureBase.Application.Dtos;
using CleanArchitectureBase.Application.Features.Base.Queries;
using CleanArchitectureBase.Application.Specifications.Base;
using CleanArchitectureBase.Application.Specifications.Translations;
using CleanArchitectureBase.Domain.Entities.Localization;

namespace CleanArchitectureBase.Application.Features.Translations.Queries.GetAllPaged
{
    public class GetAllTranslationsQuery : GetAllPagedQueryBase<TranslationDto>
    {}

    internal class GetAllProductsQueryHandler : GetAllPagedQueryHandlerBase<GetAllTranslationsQuery, int, TranslationDto, Translation>
    {
        protected override ISpecification<Translation> GetFilterSpecification(GetAllTranslationsQuery query)
        {
            return new TranslationFilterSpecification(query.SearchString);
        }

        public GetAllProductsQueryHandler(IUnitOfWork<int> unitOfWork, IMediator mediator, IServiceProvider provider) 
            : base(unitOfWork, mediator, provider)
        { }
    }
}