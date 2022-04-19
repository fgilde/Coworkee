using System.Linq;
using CleanArchitectureBase.Client.Configuration;
using CleanArchitectureBase.Client.Extensions;

namespace CleanArchitectureBase.Client.JsInterop;

public class JsNamespace
{
    public static string Get(params string[] subs)
    {
        return string.Join('.', new[] {ServiceAccessor.Get<ClientApplicationConfiguration>().JsMainNamespace}.Concat(subs));
    }
}