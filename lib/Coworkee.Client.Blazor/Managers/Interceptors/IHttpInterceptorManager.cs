using System.Threading.Tasks;
using Toolbelt.Blazor;

namespace lib.Coworkee.Client.Managers.Interceptors
{
    public interface IHttpInterceptorManager : IManager
    {
        void RegisterEvent();

        Task InterceptBeforeHttpAsync(object sender, HttpClientInterceptorEventArgs e);

        void DisposeEvent();
    }
}