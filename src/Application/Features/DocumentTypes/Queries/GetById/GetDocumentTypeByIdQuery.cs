using System;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Common.Security;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Features.Base.Queries;
using CleanArchitectureBase.Domain.Entities.Misc;
using CleanArchitectureBase.Shared.Constants.Permission;

namespace CleanArchitectureBase.Application.Features.DocumentTypes.Queries.GetById
{
    [CustomAuthorize(Policies = new[] { Permissions.DocumentTypes.View })]
    public class GetDocumentTypeByIdQuery : GetByIdQueryBase<int, DocumentTypeDto>
    {
        public GetDocumentTypeByIdQuery(int id) : base(id)
        { }
    }

    internal class GetDocumentTypeByIdQueryHandler : GetByIdQueryHandlerBase<GetDocumentTypeByIdQuery, int, DocumentTypeDto, Document>
    {
        public GetDocumentTypeByIdQueryHandler(IUnitOfWork<int> unitOfWork, IServiceProvider provider) : base(unitOfWork, provider)
        { }
    }
}