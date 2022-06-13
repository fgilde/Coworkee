using System.Linq;
using CleanArchitectureBase.Shared.Wrapper;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitectureBase.Infrastructure.Services.Identity
{
    public static class IdentityResultExtensions
    {
        public static IResult<T> ToApplicationResult<T>(this IdentityResult result, T data)
        {
            return result.Succeeded
                ? new Result<T> { Succeeded = true, Data = data }
                : new Result<T> { Succeeded = false, Data = data, Messages = result.Errors.Select(e => e.Description).ToList() };
        }

        public static IResult ToApplicationResult(this IdentityResult result)
        {
            return result.Succeeded
                ? Result.Success()
                : Result.Fail(result.Errors.Select(e => e.Description).ToList());
        }
    }
}