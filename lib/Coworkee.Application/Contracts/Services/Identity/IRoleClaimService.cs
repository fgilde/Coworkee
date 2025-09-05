using System.Collections.Generic;
using System.Threading.Tasks;
using lib.Coworkee.Application.Common.Models.Identity;
using lib.Coworkee.Application.Contracts.Common;
using lib.Coworkee.Application.Requests.Identity;
using lib.Coworkee.Shared.Wrapper;

namespace lib.Coworkee.Application.Contracts.Services.Identity
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