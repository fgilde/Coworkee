using CleanArchitectureBase.Application.Specifications.Misc;
using CleanArchitectureBase.Domain.Entities.Misc;
using MediatR;
using System;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Common.Security;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Contracts.Services;
using CleanArchitectureBase.Application.Features.Base.Queries;
using CleanArchitectureBase.Application.Specifications.Base;
using CleanArchitectureBase.Shared.Constants.Permission;

namespace CleanArchitectureBase.Application.Features.Documents.Queries.GetAll
{
    [CustomAuthorize(Policies = new[] { Permissions.Documents.View })]
    public class GetAllDocumentsQuery : GetAllPagedQueryBase<DocumentDto>
    { }

    internal class GetAllDocumentsQueryHandler : GetAllPagedQueryHandlerBase<GetAllDocumentsQuery, int, DocumentDto, Document>
    {
        private readonly ICurrentUserService _currentUserService;

        protected override ISpecification<Document> GetFilterSpecification(GetAllDocumentsQuery query)
        {
            return new DocumentFilterSpecification(query.SearchString, _currentUserService.UserId);
        }

        public GetAllDocumentsQueryHandler(IUnitOfWork<int> unitOfWork, IMediator mediator, IServiceProvider provider, ICurrentUserService currentUserService)
            : base(unitOfWork, mediator, provider)
        {
            _currentUserService = currentUserService;
        }
    }
}