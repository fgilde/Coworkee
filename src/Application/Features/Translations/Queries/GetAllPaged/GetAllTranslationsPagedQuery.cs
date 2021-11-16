using System;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Common.Security;
using CleanArchitectureBase.Application.Contracts.Repositories;
using MediatR;
using CleanArchitectureBase.Application.Features.Base.Queries;
using CleanArchitectureBase.Application.Specifications.Base;
using CleanArchitectureBase.Application.Specifications.Translations;
using CleanArchitectureBase.Domain.Entities.Localization;
using CleanArchitectureBase.Shared.Constants.Permission;

namespace CleanArchitectureBase.Application.Features.Translations.Queries.GetAllPaged
{
    [CustomAuthorize(Policies = new[] { Permissions.Translations.View })]
    public class GetAllTranslationsPagedQuery : GetAllPagedQueryBase<TranslationDto>
    {}

    internal class GetAllTranslationsPagedQueryHandler : GetAllPagedQueryHandlerBase<GetAllTranslationsPagedQuery, int, TranslationDto, Translation>
    {
        protected override ISpecification<Translation> GetFilterSpecification(GetAllTranslationsPagedQuery query)
        {
            return new TranslationFilterSpecification(query.SearchString);
        }

        public GetAllTranslationsPagedQueryHandler(IUnitOfWork<int> unitOfWork, IMediator mediator, IServiceProvider provider) 
            : base(unitOfWork, mediator, provider)
        { }
    }
}