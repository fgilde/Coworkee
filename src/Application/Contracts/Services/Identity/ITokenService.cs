using System.Threading.Tasks;
using Coworkee.Application.Common.Models.Identity;
using Coworkee.Application.Contracts.Common;
using Coworkee.Application.Requests.Identity;
using Coworkee.Shared.Wrapper;

namespace Coworkee.Application.Contracts.Services.Identity
{
    public interface ITokenService : IService
    {
        Task<Result<TokenResponse>> LoginAsync(TokenRequest model);
        Task<Result<TokenResponse>> RegenerateTokenAsync(string[] specificRoles);
        Task<Result<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest model);
        Task<string> GenerateTokenForUser(string userId);
    }
}