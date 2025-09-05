using System.Threading.Tasks;
using lib.Coworkee.Application.Common.Models.Identity;
using lib.Coworkee.Application.Contracts.Common;
using lib.Coworkee.Application.Requests.Identity;
using lib.Coworkee.Shared.Wrapper;

namespace lib.Coworkee.Application.Contracts.Services.Account
{
    public interface IAccountService : IService
    {
        Task<IResult> ChangePasswordAsync(ChangePasswordRequest model, string userId);

        Task<IResult<string>> GetProfilePictureAsync(string userId);

        Task<IResult<TokenResponse>> UpdateProfilePictureAsync(UpdateProfilePictureRequest request, string userId);

        Task<string> GetUserNameAsync(string userId);

        Task<bool> IsInRoleAsync(string userId, string role);

        Task<bool> AuthorizeAsync(string userId, string policyName);

        Task<(IResult Result, string UserId)> CreateUserAsync(string userName, string password);

        Task<IResult> DeleteUserAsync(string userId);

        Task WithRoles(params string[] roles);

        Task WithPermissions(params string[] permissions);

        Task LogoutAsync();
    }
}