using CleanArchitectureBase.Application.Features.Documents.Commands.AddEdit;
using CleanArchitectureBase.Application.Features.Documents.Queries.GetAll;
using CleanArchitectureBase.Shared.Wrapper;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Features.Documents.Queries.GetById;

namespace CleanArchitectureBase.Client.Infrastructure.Managers.Misc.Document
{
    public interface IDocumentManager : IManager
    {
        Task<PaginatedResult<GetAllDocumentsResponse>> GetAllAsync(GetAllDocumentsQuery query);

        Task<IResult<GetDocumentByIdResponse>> GetByIdAsync(GetDocumentByIdQuery request);

        Task<IResult<int>> SaveAsync(AddEditDocumentCommand request);

        Task<IResult<int>> DeleteAsync(int id);
    }
}