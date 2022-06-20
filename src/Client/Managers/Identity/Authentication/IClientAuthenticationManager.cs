using System.Security.Claims;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models.Identity;
using CleanArchitectureBase.Application.Requests.Identity;
using CleanArchitectureBase.Shared.Wrapper;

namespace CleanArchitectureBase.Client.Managers.Identity.Authentication
{
    public interface IClientAuthenticationManager : IManager
    {
        Task<IResult> Login(TokenRequest model);
        Task<IResult> RegenerateAndUpdateTokenAsync();
        Task UpdateToken(TokenResponse response);
        Task<IResult> Logout();
        Task<string> RefreshToken();
        Task<string> TryRefreshToken();
        Task<string> TryForceRefreshToken();
        Task<ClaimsPrincipal> CurrentUser();
    }
}