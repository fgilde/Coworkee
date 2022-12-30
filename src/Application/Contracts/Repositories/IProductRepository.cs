using System.Threading.Tasks;

namespace Coworkee.Application.Contracts.Repositories
{
    public interface IProductRepository
    {
        Task<bool> IsBrandUsed(int brandId);
    }
}