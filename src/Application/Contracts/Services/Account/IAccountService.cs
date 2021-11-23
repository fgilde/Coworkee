using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models.Identity;
using CleanArchitectureBase.Application.Contracts.Common;
using CleanArchitectureBase.Application.Requests.Identity;
using CleanArchitectureBase.Shared.Wrapper;

namespace CleanArchitectureBase.Application.Contracts.Services.Account
{
    public interface IAccountService : IService
    {
        Task<string> UpdateProfileAsync(UpdateProfileRequest model, string userId);

        Task<IResult> ChangePasswordAsync(ChangePasswordRequest model, string userId);

        Task<IResult<string>> GetProfilePictureAsync(string userId);

        Task<IResult<TokenResponse>> UpdateProfilePictureAsync(UpdateProfilePictureRequest request, string userId);

        Task<string> GetUserNameAsync(string userId);

        Task<bool> IsInRoleAsync(string userId, string role);

        Task<bool> AuthorizeAsync(string userId, string policyName);

        Task<(IResult Result, string UserId)> CreateUserAsync(string userName, string password);

        Task<IResult> DeleteUserAsync(string userId);
        Task LogoutAsync();
    }
}