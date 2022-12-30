using System.Collections.Generic;
using System.Threading.Tasks;
using Coworkee.Application.Common.Models.Identity;
using Coworkee.Application.Contracts.Common;
using Coworkee.Application.Requests.Identity;
using Coworkee.Shared.Wrapper;

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