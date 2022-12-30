using System.Security.Claims;
using System.Threading.Tasks;
using Coworkee.Application.Common.Models.Identity;
using Coworkee.Application.Requests.Identity;
using Coworkee.Shared.Wrapper;

namespace Coworkee.Client.Managers.Identity.Authentication
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