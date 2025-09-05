using lib.Coworkee.Application.Contracts.Repositories;
using lib.Coworkee.Domain.Entities.Catalog;

namespace Coworkee.Infrastructure.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        private readonly IRepositoryAsync<Brand, int> _repository;

        public BrandRepository(IRepositoryAsync<Brand, int> repository)
        {
            _repository = repository;
        }
    }
}