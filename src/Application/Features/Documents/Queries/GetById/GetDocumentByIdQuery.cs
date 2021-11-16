using System;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Common.Security;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Features.Base.Queries;
using CleanArchitectureBase.Domain.Entities.Misc;
using CleanArchitectureBase.Shared.Constants.Permission;

namespace CleanArchitectureBase.Application.Features.Documents.Queries.GetById
{
    [CustomAuthorize(Policies = new[] { Permissions.Documents.View })]
    public class GetDocumentByIdQuery : GetByIdQueryBase<int, DocumentDto>
    {
        public GetDocumentByIdQuery(int id) : base(id)
        { }
    }

    internal class GetDocumentByIdQueryHandler : GetByIdQueryHandlerBase<GetDocumentByIdQuery, int, DocumentDto, Document>
    {
        public GetDocumentByIdQueryHandler(IUnitOfWork<int> unitOfWork, IServiceProvider provider) : base(unitOfWork, provider)
        { }
    }
}