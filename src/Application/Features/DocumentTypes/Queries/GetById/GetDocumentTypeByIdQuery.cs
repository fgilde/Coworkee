using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Interfaces.Repositories;
using CleanArchitectureBase.Domain.Entities.Misc;
using CleanArchitectureBase.Shared.Wrapper;
using MediatR;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Application.Features.DocumentTypes.Queries.GetById
{
    public class GetDocumentTypeByIdQuery : IRequest<Result<GetDocumentTypeByIdResponse>>
    {
        public int Id { get; set; }
    }

    internal class GetDocumentTypeByIdQueryHandler : IRequestHandler<GetDocumentTypeByIdQuery, Result<GetDocumentTypeByIdResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;

        public GetDocumentTypeByIdQueryHandler(IUnitOfWork<int> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<GetDocumentTypeByIdResponse>> Handle(GetDocumentTypeByIdQuery query, CancellationToken cancellationToken)
        {
            var documentType = await _unitOfWork.Repository<DocumentType>().GetByIdAsync(query.Id);
            var mappedDocumentType = documentType.MapTo<GetDocumentTypeByIdResponse>();
            return await Result<GetDocumentTypeByIdResponse>.SuccessAsync(mappedDocumentType);
        }
    }
}