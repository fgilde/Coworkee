using CleanArchitectureBase.Application.Specifications.Misc;
using CleanArchitectureBase.Domain.Entities.Misc;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using HeyRed.Mime;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Common.Security;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Contracts.Services;
using CleanArchitectureBase.Application.Features.Base.Queries;
using CleanArchitectureBase.Application.Specifications.Base;
using CleanArchitectureBase.Shared.Constants.Permission;
using CleanArchitectureBase.Shared.Constants.Role;
using CleanArchitectureBase.Shared.Wrapper;

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
            var isAdmin = _currentUserService.Principal.IsInRole(RoleConstants.AdministratorRole);
            return new DocumentFilterSpecification(query.SearchString, _currentUserService.UserId, isAdmin);
        }

        public GetAllDocumentsQueryHandler(IUnitOfWork<int> unitOfWork, IMediator mediator, IServiceProvider provider, ICurrentUserService currentUserService)
            : base(unitOfWork, mediator, provider)
        {
            _currentUserService = currentUserService;
        }

        public override async Task<PaginatedResult<DocumentDto>> Handle(GetAllDocumentsQuery request, CancellationToken cancellationToken)
        {
            var res = await base.Handle(request, cancellationToken);
            res.Data.ForEach(dto => // TODO
            {
                dto.ContentType = MimeTypesMap.GetMimeType(dto.URL);
            });

            return res;
        }
    }
}