using System;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Common.Security;
using Coworkee.Application.Contracts.Repositories;
using MediatR;
using Coworkee.Application.Features.Base.Queries;
using Coworkee.Application.Specifications.Base;
using Coworkee.Application.Specifications.Translations;
using Coworkee.Domain.Entities.Localization;
using Coworkee.Shared.Constants.Permission;

namespace Coworkee.Application.Features.Translations.Queries.GetAllPaged
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