using System.Threading.Tasks;

namespace CleanArchitectureBase.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<bool> IsBrandUsed(int brandId);
    }
}