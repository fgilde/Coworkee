using System.Threading.Tasks;
using CleanArchitectureBase.Application.Contracts.Services;
using CleanArchitectureBase.Application.Contracts.Services.Account;
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

    public static async Task<IServiceScope> AsUserAsync(this IServiceScope scope, string userId)
    {
        var cu = scope.ServiceProvider.GetRequiredService<ICurrentUserService>();
        await cu.AsUser(userId);
        return scope;
    }

    public static async Task<IServiceScope> WithPermissions(this Task<IServiceScope> scope, params string[] permissions)
    {
        return await (await scope).WithPermissions(permissions);
    }
    public static async Task<IServiceScope> WithPermissions(this IServiceScope scope, params string[] permissions)
    {
        var accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();
        await accountService.WithPermissions(permissions);
        return scope;
    }

    public static async Task<IServiceScope> WithRoles(this Task<IServiceScope> scope, params string[] roles)
    {
        return await (await scope).WithRoles(roles);
    }
    public static async Task<IServiceScope> WithRoles(this IServiceScope scope, params string[] roles)
    {
        var accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();
        await accountService.WithRoles(roles);
        return scope;
    }
}