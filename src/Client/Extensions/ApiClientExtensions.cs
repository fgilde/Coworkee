using System;
using System.Threading.Tasks;
using CleanArchitectureBase.Client.ErrorHandling;
using CleanArchitectureBase.SDK;
using CleanArchitectureBase.Shared.Wrapper;
using SDK;

namespace CleanArchitectureBase.Client.Extensions
{
    public static class ApiClientExtensions
    {
        //internal static ErrorHandler Handler { get; } = ServiceAccessor.Get<ErrorHandler>();

        //public static async Task<TResult> CallAsync<TResult>(this IBlazorHeroClient api, Func<IBlazorHeroClient, Task<TResult>> action)
        //{
        //    try
        //    {
        //        var result = await action(api);
        //        if (result is IResult {Succeeded: false} asIResult)
        //            Handler.ShowErrors(asIResult.Messages);
        //        //else if (result is FileResponse {StatusCode: < 200 or > 299} response)
        //        //{
                    
        //        //}
        //        else
        //            return result;
        //    }
        //    catch (Exception e)
        //    {
        //        Handler.ShowException(e);
        //    }
        //    return default;
        //}
    }
}