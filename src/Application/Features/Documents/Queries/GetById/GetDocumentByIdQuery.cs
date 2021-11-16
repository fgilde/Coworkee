using System;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Features.Base.Queries;
using CleanArchitectureBase.Domain.Entities.Misc;

namespace CleanArchitectureBase.Application.Features.Documents.Queries.GetById
{
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