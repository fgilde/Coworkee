using System.Collections.Generic;
using System.Threading.Tasks;
using lib.Coworkee.Application.Common.Models.Identity;
using lib.Coworkee.Application.Contracts.Common;
using lib.Coworkee.Application.Requests.Identity;
using lib.Coworkee.Shared.Wrapper;

namespace Coworkee.Application.Contracts.Services.Identity
{
    public interface IRoleService : IService
    {
        Task<Result<List<RoleDto>>> GetAllAsync();

        Task<int> GetCountAsync();

        Task<Result<RoleDto>> GetByIdAsync(string id);

        Task<Result<string>> SaveAsync(RoleDto request);

        Task<Result<string>> DeleteAsync(string id);

        Task<Result<PermissionResponse>> GetAllPermissionsAsync(string roleId);

        Task<Result<string>> UpdatePermissionsAsync(PermissionRequest request);
    }
}