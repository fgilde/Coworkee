using lib.Coworkee.Application.Specifications.Misc;
using lib.Coworkee.Domain.Entities.Misc;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using lib.Coworkee.Application.Common.Models;
using lib.Coworkee.Application.Common.Security;
using lib.Coworkee.Application.Contracts.Repositories;
using lib.Coworkee.Application.Contracts.Services;
using lib.Coworkee.Shared.Constants.Role;
using lib.Coworkee.Application.Features.Base.Queries;
using lib.Coworkee.Application.Specifications.Base;
using lib.Coworkee.Shared.Constants.Permission;
using lib.Coworkee.Shared.Constants.Role;
using lib.Coworkee.Shared.Wrapper;
using Nextended.Core;

namespace lib.Coworkee.Application.Features.Documents.Queries.GetAll
{
    [CustomAuthorize(Policies = new[] { CorePermissionProvider.Core.Documents.View })]
    public class GetAllDocumentsQuery : GetAllPagedQueryBase<DocumentDto>
    { }

    internal class GetAllDocumentsQueryHandler : GetAllPagedQueryHandlerBase<GetAllDocumentsQuery, int, DocumentDto, Document>
    {
        private readonly ICurrentUserService _currentUserService;

        protected override ISpecification<Document> GetFilterSpecification(GetAllDocumentsQuery query)
        {
            var isAdmin = _currentUserService.Principal.IsInRole(CoreRoleConstants.AdministratorRole);
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
                dto.ContentType = MimeType.GetMimeType(dto.URL);
            });

            return res;
        }
    }
}