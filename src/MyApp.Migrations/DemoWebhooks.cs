using Coworkee.Application.Messaging;
using Coworkee.Application.Setup;
using Coworkee.Contracts.Webhooks;
using Coworkee.Core.Results;
using Coworkee.Core.Security;
using Coworkee.Identity.Domain;
using Coworkee.Webhooks;
using Microsoft.EntityFrameworkCore;
using MyApp.Infrastructure;
using Nextended.Core.Attributes;

namespace MyApp.Migrations;

/// <summary>One switched off example endpoint per database, so the webhooks page shows how an endpoint looks; it never sends.</summary>
[RegisterAs(typeof(ISeedContributor))]
internal sealed class DemoWebhooks(MyAppDbContext db, ITenantDirectory tenants, IServiceProvider services) : ISeedContributor
{
    public const string ExampleUrl = "https://example.org/hook";

    public int Order => DemoSeed.Order.Webhooks;

    public SeedEnvironments Environments => SeedEnvironments.Development;

    public async Task SeedAsync(SeedContext context, CancellationToken cancellationToken)
    {
        if (!await DemoSeed.IsSystemTenantAsync(context, tenants, cancellationToken))
        {
            return;
        }

        var tenantId = context.TenantId!.Value;
        if (await db.Set<WebhookSubscription>().IgnoreQueryFilters().AnyAsync(w => w.TenantId == tenantId, cancellationToken)
            || await db.Set<User>().IgnoreQueryFilters().Where(u => u.TenantId == tenantId && u.Email == DemoUsers.Administrator.Email).Select(u => (Guid?)u.Id)
                .SingleOrDefaultAsync(cancellationToken) is not { } admin)
        {
            return;
        }

        // as the administrator, like a click in the app; the command commits in its own scope
        using var actor = CurrentUserScope.Begin(new ImpersonatedUser(admin, tenantId));
        await using var command = services.CreateAsyncScope();
        await command.ServiceProvider.GetRequiredService<IDispatcher>().SendAsync(new CreateWebhookCommand(new WebhookRequest(
            ExampleUrl, ["products.*", "brands.*", "sales_orders.*"], "Example endpoint: switch it on with a URL of yours", IsActive: false)), cancellationToken).OrThrow();
    }
}
