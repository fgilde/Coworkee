using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Coworkee.Application.Common.Models.Identity;
using Coworkee.Application.Contracts.Common;
using Coworkee.Application.Hubs.Events.Base;
using Coworkee.Application.Requests.Identity;
using Coworkee.Shared.Models;
using Coworkee.Shared.Wrapper;
using Microsoft.AspNetCore.Identity;

namespace Coworkee.Application.Contracts.Services.Identity
{
    public interface IUserService : IService
    {
        Task<UserResponse> SystemUserAsync();
        Task<IEnumerable<UserResponse>> GetAllForTargetAsync(EventTarget eventTarget);
        Task<UserResponse[]> GetOrAddUserAsync(params CreateUser[] user);
        Task<Result<List<UserResponse>>> GetAllAsync();
        Task<int> GetCountAsync();
        Task<IResult<UserResponse>> GetAsync(string userId);
        UserResponse Get(string userId);
        Task<IResult> RegisterAsync(RegisterRequest request, string origin);
        Task<IResult> ToggleUserStatusAsync(ToggleUserStatusRequest request);
        Task<IResult<UserRolesResponse>> GetRolesAsync(string id = null);
        Task<IResult> UpdateRolesAsync(UpdateUserRolesRequest request);
        Task<IResult<string>> UpdateUserAsync(UserResponse user);
        Task<IResult<string>> ConfirmEmailAsync(string userId, string code);
        Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request, string origin);
        Task<IResult> ResetPasswordAsync(ResetPasswordRequest request);
        Task<string> ExportToExcelAsync(string searchString = "");
        Task<IResult> DeleteAsync(string userId, CancellationToken cancellationToken = default);
        Task<IdentityResult> SetUserCulture(string userId, CultureInfo culture);
    }
}