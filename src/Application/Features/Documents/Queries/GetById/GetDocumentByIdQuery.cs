using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Domain.Entities.Misc;
using CleanArchitectureBase.Shared.Wrapper;
using MediatR;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Application.Features.Documents.Queries.GetById
{
    public class GetDocumentByIdQuery : IRequest<Result<GetDocumentByIdResponse>>
    {
        public int Id { get; set; }
    }

    internal class GetDocumentByIdQueryHandler : IRequestHandler<GetDocumentByIdQuery, Result<GetDocumentByIdResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        
        public GetDocumentByIdQueryHandler(IUnitOfWork<int> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<GetDocumentByIdResponse>> Handle(GetDocumentByIdQuery query, CancellationToken cancellationToken)
        {
            var document = await _unitOfWork.Repository<Document>().GetByIdAsync(query.Id);
            var mappedDocument = document.MapTo<GetDocumentByIdResponse>();
            return await Result<GetDocumentByIdResponse>.SuccessAsync(mappedDocument);
        }
    }
}