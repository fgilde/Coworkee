using System.Collections.Generic;
using System.Threading.Tasks;
using Coworkee.Application.Common.Models.Identity;
using Coworkee.Application.Contracts.Common;
using Coworkee.Application.Requests.Identity;
using Coworkee.Shared.Wrapper;

namespace Coworkee.Application.Contracts.Services.Identity
{
    public interface IRoleClaimService : IService
    {
        Task<Result<List<RoleClaimResponse>>> GetAllAsync();

        Task<int> GetCountAsync();

        Task<Result<RoleClaimResponse>> GetByIdAsync(int id);

        Task<Result<List<RoleClaimResponse>>> GetAllByRoleIdAsync(string roleId);

        Task<Result<string>> SaveAsync(RoleClaimRequest request);

        Task<Result<string>> DeleteAsync(int id);
    }
}