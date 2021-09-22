using CleanArchitectureBase.Shared.Wrapper;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Features.Dashboards.Queries.GetData;

namespace CleanArchitectureBase.Client.Infrastructure.Managers.Dashboard
{
    public interface IDashboardManager : IManager
    {
        Task<IResult<DashboardDataResponse>> GetDataAsync();
    }
}