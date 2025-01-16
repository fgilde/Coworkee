using System.Security.Claims;
using System.Threading.Tasks;
using Coworkee.Application.Common.Models.Identity;
using Coworkee.Application.Contracts.Common;
using Coworkee.Application.Requests.Identity;
using Coworkee.Shared.Wrapper;

namespace Coworkee.Application.Contracts.Services.Identity
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