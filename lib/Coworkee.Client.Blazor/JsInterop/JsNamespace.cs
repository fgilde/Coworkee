using System.Linq;
using lib.Coworkee.Client.Configuration;
using lib.Coworkee.Client.Extensions;

namespace lib.Coworkee.Client.JsInterop;

public class JsNamespace
{
    public static string Get(params string[] subs)
    {
        return string.Join('.', new[] {ServiceAccessor.Get<ClientApplicationConfiguration>().JsMainNamespace}.Concat(subs));
    }
}