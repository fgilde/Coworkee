using System.Threading.Tasks;

namespace CleanArchitectureBase.Application.Contracts.Repositories
{
    public interface IProductRepository
    {
        Task<bool> IsBrandUsed(int brandId);
    }
}