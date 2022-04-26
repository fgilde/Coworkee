using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models.Identity;
using CleanArchitectureBase.Application.Contracts.Common;
using CleanArchitectureBase.Application.Hubs.Events.Base;
using CleanArchitectureBase.Application.Requests.Identity;
using CleanArchitectureBase.Shared.Wrapper;

namespace CleanArchitectureBase.Application.Contracts.Services.Identity
{
    public interface IUserService : IService
    {
        Task<IEnumerable<UserResponse>> GetAllForTargetAsync(EventTarget eventTarget);

        Task<Result<List<UserResponse>>> GetAllAsync();

        Task<int> GetCountAsync();

        Task<IResult<UserResponse>> GetAsync(string userId);
        UserResponse Get(string userId);

        Task<IResult> RegisterAsync(RegisterRequest request, string origin);

        Task<IResult> ToggleUserStatusAsync(ToggleUserStatusRequest request);

        Task<IResult<UserRolesResponse>> GetRolesAsync(string id = null);

        Task<IResult> UpdateRolesAsync(UpdateUserRolesRequest request);

        Task<IResult<string>> ConfirmEmailAsync(string userId, string code);

        Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request, string origin);

        Task<IResult> ResetPasswordAsync(ResetPasswordRequest request);

        Task<string> ExportToExcelAsync(string searchString = "");

        Task<IResult> DeleteAsync(string userId, CancellationToken cancellationToken = default);
    }
}