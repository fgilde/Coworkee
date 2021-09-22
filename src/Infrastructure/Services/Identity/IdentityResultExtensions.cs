using System.Linq;
using CleanArchitectureBase.Shared.Wrapper;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitectureBase.Infrastructure.Services.Identity
{
    public static class IdentityResultExtensions
    {
        public static IResult ToApplicationResult(this IdentityResult result)
        {
            return result.Succeeded
                ? Result.Success()
                : Result.Fail(result.Errors.Select(e => e.Description).ToList());
        }
    }
}