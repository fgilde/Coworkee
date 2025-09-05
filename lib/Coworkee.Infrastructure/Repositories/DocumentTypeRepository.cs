using lib.Coworkee.Application.Contracts.Repositories;
using lib.Coworkee.Domain.Entities.Misc;

namespace lib.Coworkee.Infrastructure.Repositories
{
    public class DocumentTypeRepository : IDocumentTypeRepository
    {
        private readonly IRepositoryAsync<DocumentType, int> _repository;

        public DocumentTypeRepository(IRepositoryAsync<DocumentType, int> repository)
        {
            _repository = repository;
        }
    }
}