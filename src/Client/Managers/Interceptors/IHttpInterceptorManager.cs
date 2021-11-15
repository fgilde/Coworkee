using System.Threading.Tasks;
using Toolbelt.Blazor;

namespace CleanArchitectureBase.Client.Managers.Interceptors
{
    public interface IHttpInterceptorManager : IManager
    {
        void RegisterEvent();

        Task InterceptBeforeHttpAsync(object sender, HttpClientInterceptorEventArgs e);

        void DisposeEvent();
    }
}