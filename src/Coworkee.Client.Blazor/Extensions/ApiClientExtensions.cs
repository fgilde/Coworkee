using System;
using System.Threading.Tasks;
using Coworkee.Client.ErrorHandling;
using Coworkee.SDK;
using Coworkee.Shared.Wrapper;
using SDK;

namespace Coworkee.Client.Extensions
{
    public static class ApiClientExtensions
    {
        //internal static ErrorHandler Handler { get; } = ServiceAccessor.Get<ErrorHandler>();

        //public static async Task<TResult> CallAsync<TResult>(this IApplicationClient api, Func<IApplicationClient, Task<TResult>> action)
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