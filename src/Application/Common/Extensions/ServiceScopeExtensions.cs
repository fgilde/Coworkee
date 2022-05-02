using System.Threading.Tasks;
using CleanArchitectureBase.Application.Contracts.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureBase.Application.Common.Extensions;

public static class ServiceScopeExtensions
{
    public static async Task<IServiceScope> AsSystemUserAsync(this IServiceScope scope)
    {
        var cu = scope.ServiceProvider.GetService<ICurrentUserService>();
        await cu.AsSystemUser();
        return scope;
    }
}