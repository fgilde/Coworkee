using System.Security.Claims;
using System.Threading.Tasks;
using lib.Coworkee.Application.Common.Models.Identity;
using lib.Coworkee.Application.Contracts.Common;
using lib.Coworkee.Application.Requests.Identity;
using lib.Coworkee.Shared.Wrapper;

namespace lib.Coworkee.Application.Contracts.Services.Identity
{
    public interface ITokenService : IService
    {
        /// <summary>
        /// Login user with external provider the user will then searched by email or name and signed in if exists.
        /// </summary>
        Task<Result<TokenResponse>> LoginExternalAsync(ClaimsPrincipal externalClaim, ExternalLoginOptions options);
        Task<Result<TokenResponse>> LoginAsync(TokenRequest model);
        Task<Result<TokenResponse>> RegenerateTokenAsync(string[] specificRoles);
        Task<Result<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest model);
        Task<string> GenerateTokenForUser(string userId);
    }
}