using System;
using lib.Coworkee.Application.Common.Models;
using lib.Coworkee.Application.Common.Security;
using lib.Coworkee.Application.Contracts.Repositories;
using MediatR;
using lib.Coworkee.Application.Features.Base.Queries;
using lib.Coworkee.Application.Specifications.Base;
using lib.Coworkee.Application.Specifications.Translations;
using lib.Coworkee.Domain.Entities.Localization;
using lib.Coworkee.Shared.Constants.Permission;

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