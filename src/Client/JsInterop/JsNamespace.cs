using System.Linq;
using Coworkee.Client.Configuration;
using Coworkee.Client.Extensions;

namespace Coworkee.Client.JsInterop;

public class JsNamespace
{
    public static string Get(params string[] subs)
    {
        return string.Join('.', new[] {ServiceAccessor.Get<ClientApplicationConfiguration>().JsMainNamespace}.Concat(subs));
    }
}