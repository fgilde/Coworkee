using Coworkee.Application.Messaging;
using Coworkee.Contracts.Webhooks;
using Coworkee.Core.Results;
using Coworkee.Core.Security;
using Coworkee.Identity.Domain;
using Coworkee.Webhooks;
using Microsoft.EntityFrameworkCore;
using MyApp.Infrastructure;

namespace MyApp.Migrations;

/// <summary>One switched off example endpoint per database, so the webhooks page shows how an endpoint looks; it never sends.</summary>
internal static class DemoWebhooks
{
    public const string ExampleUrl = "https://example.org/hook";

    public static async Task EnsureAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var provider = scope.ServiceProvider;
        if (await provider.GetRequiredService<ITenantDirectory>().GetSystemTenantIdAsync(cancellationToken) is not { } tenantId)
        {
            return;
        }

        var db = provider.GetRequiredService<MyAppDbContext>();
        if (await db.Set<WebhookSubscription>().IgnoreQueryFilters().AnyAsync(w => w.TenantId == tenantId, cancellationToken)
            || await db.Set<User>().IgnoreQueryFilters().Where(u => u.TenantId == tenantId && u.Email == DemoUsers.Administrator.Email).Select(u => (Guid?)u.Id)
                .SingleOrDefaultAsync(cancellationToken) is not { } admin)
        {
            return;
        }

        using var actor = CurrentUserScope.Begin(new ImpersonatedUser(admin, tenantId));
        await using var command = services.CreateAsyncScope();
        await command.ServiceProvider.GetRequiredService<IDispatcher>().SendAsync(new CreateWebhookCommand(new WebhookRequest(
            ExampleUrl, ["products.*", "brands.*", "sales_orders.*"], "Example endpoint: switch it on with a URL of yours", IsActive: false)), cancellationToken).OrThrow();
    }
}
