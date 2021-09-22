using CleanArchitectureBase.Application.Features.Documents.Commands.AddEdit;
using CleanArchitectureBase.Application.Features.Documents.Queries.GetAll;
using CleanArchitectureBase.Client.Infrastructure.Extensions;
using CleanArchitectureBase.Shared.Wrapper;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Features.Documents.Queries.GetById;

namespace CleanArchitectureBase.Client.Infrastructure.Managers.Misc.Document
{
    public class DocumentManager : IDocumentManager
    {
        private readonly HttpClient _httpClient;

        public DocumentManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<int>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{Routes.DocumentsEndpoints.Delete}/{id}");
            return await response.ToResult<int>();
        }

        public async Task<PaginatedResult<GetAllDocumentsResponse>> GetAllAsync(GetAllDocumentsQuery query)
        {
            var response = await _httpClient.GetAsync(Routes.DocumentsEndpoints.GetAllPaged(query));
            return await response.ToPaginatedResult<GetAllDocumentsResponse>();
        }

        public async Task<IResult<GetDocumentByIdResponse>> GetByIdAsync(GetDocumentByIdQuery request)
        {
            var response = await _httpClient.GetAsync(Routes.DocumentsEndpoints.GetById(request.Id));
            return await response.ToResult<GetDocumentByIdResponse>();
        }

        public async Task<IResult<int>> SaveAsync(AddEditDocumentCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.DocumentsEndpoints.Save, request);
            return await response.ToResult<int>();
        }
    }
}