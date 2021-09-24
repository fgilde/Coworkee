using CleanArchitectureBase.Application.Requests.Identity;
using CleanArchitectureBase.Shared.Wrapper;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CleanArchitectureBase.Client.Infrastructure.Managers.Identity.Authentication
{
    public interface IClientAuthenticationManager : IManager
    {
        Task<IResult> Login(TokenRequest model);

        Task<IResult> Logout();

        Task<string> RefreshToken();

        Task<string> TryRefreshToken();

        Task<string> TryForceRefreshToken();

        Task<ClaimsPrincipal> CurrentUser();
    }
}