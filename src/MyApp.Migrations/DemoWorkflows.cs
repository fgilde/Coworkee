using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using Coworkee.Core.Security;
using Coworkee.Identity.Domain;
using Coworkee.Contracts.Workflows;
using Coworkee.Workflows;
using Microsoft.EntityFrameworkCore;
using MyApp.Catalog.Domain;
using MyApp.Catalog.Features.Products;
using MyApp.Contracts.Sales;
using MyApp.Infrastructure;
using MyApp.Sales.Domain;
using MyApp.Sales.Features.Orders;

namespace MyApp.Migrations;

/// <summary>
/// Approvals in every state, once per database, so the approvals inbox and the timelines have something to show: John (product manager) submits
/// six products, an administrator approves two and rejects one; three large orders wait for approval and one small one is confirmed.
/// Everything runs through the commands as the users, like clicks in the app.
/// </summary>
internal static class DemoWorkflows
{
    public static async Task EnsureAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var provider = scope.ServiceProvider;
        if (await provider.GetRequiredService<ITenantDirectory>().GetSystemTenantIdAsync(cancellationToken) is not { } tenantId)
        {
            return;
        }

        var db = provider.GetRequiredService<MyAppDbContext>();
        if (await db.Set<WorkflowItem>().IgnoreQueryFilters().AnyAsync(i => i.TenantId == tenantId, cancellationToken))
        {
            return;
        }

        var users = await db.Set<User>().IgnoreQueryFilters().Where(u => u.TenantId == tenantId).ToDictionaryAsync(u => u.Email!, u => u.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);
        if (!users.TryGetValue(DemoUsers.John.Email, out var john) || !users.TryGetValue(DemoUsers.Administrator.Email, out var admin) || !users.TryGetValue(DemoUsers.Florian.Email, out var florian))
        {
            return;
        }

        var products = await db.Set<Product>().IgnoreQueryFilters().Where(p => p.TenantId == tenantId && p.Rate > 0).OrderBy(p => p.Name).Select(p => p.Id).Take(6).ToListAsync(cancellationToken);
        foreach (var product in products)
        {
            await SendAsync(services, john, tenantId, new RunTransitionCommand(ProductApproval.EntityType, product, "Submit", "Ready for the catalog"), cancellationToken);
        }

        for (var i = 3; i < products.Count; i++)
        {
            var approval = await OpenApprovalAsync(services, tenantId, products[i], cancellationToken);
            var approve = i < 5;
            await SendAsync(services, admin, tenantId, new DecideApprovalCommand(approval, approve, approve ? "Looks good" : "The image is missing"), cancellationToken);
        }

        var orders = await db.Set<SalesOrder>().IgnoreQueryFilters()
            .Where(o => o.TenantId == tenantId && o.Status == SalesOrderStatus.Open).OrderByDescending(o => o.OrderDate)
            .Select(o => new { o.Id, o.Total }).Take(200).ToListAsync(cancellationToken);
        foreach (var order in orders.Where(o => o.Total >= OrderApproval.LargeTotal).Take(3))
        {
            await SendAsync(services, florian, tenantId, new RunTransitionCommand(OrderApproval.EntityType, order.Id, "Request approval", "Large order"), cancellationToken);
        }

        if (orders.FirstOrDefault(o => o.Total < OrderApproval.LargeTotal) is { } small)
        {
            await SendAsync(services, florian, tenantId, new RunTransitionCommand(OrderApproval.EntityType, small.Id, "Confirm"), cancellationToken);
        }
    }

    private static async Task SendAsync(IServiceProvider services, Guid userId, Guid tenantId, RunTransitionCommand command, CancellationToken cancellationToken) =>
        await Dispatch(services, userId, tenantId, command, cancellationToken);

    private static async Task SendAsync(IServiceProvider services, Guid userId, Guid tenantId, DecideApprovalCommand command, CancellationToken cancellationToken) =>
        await Dispatch(services, userId, tenantId, command, cancellationToken);

    private static async Task Dispatch<T>(IServiceProvider services, Guid userId, Guid tenantId, IRequest<Result<T>> command, CancellationToken cancellationToken)
    {
        using var actor = CurrentUserScope.Begin(new ImpersonatedUser(userId, tenantId));
        await using var scope = services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<IDispatcher>().SendAsync(command, cancellationToken).OrThrow();
    }

    private static async Task<Guid> OpenApprovalAsync(IServiceProvider services, Guid tenantId, Guid entityId, CancellationToken cancellationToken)
    {
        using var actor = CurrentUserScope.Begin(new ImpersonatedUser(null, tenantId));
        await using var scope = services.CreateAsyncScope();
        return await scope.ServiceProvider.GetRequiredService<MyAppDbContext>().Set<WorkflowApproval>()
            .Where(a => a.EntityId == entityId && a.Status == ApprovalStatus.Pending).Select(a => a.Id).SingleAsync(cancellationToken);
    }
}
