using Coworkee.Application.Setup;
using Coworkee.Contracts.Identity;
using Coworkee.Core.Security;
using Coworkee.Identity.Domain;
using Coworkee.Identity.Setup;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyApp.Infrastructure;
using Nextended.Core.Attributes;

namespace MyApp.Migrations;

/// <summary>
/// The identity seed runs only on a fresh database; this keeps its roles on every development run: a missing role comes with its permissions, an
/// existing one only gets the registration flag. Users, passwords and the demo data stay untouched.
/// </summary>
[RegisterAs(typeof(ISeedContributor))]
internal sealed class DemoRoles(MyAppDbContext db, ITenantDirectory tenants, IOptions<IdentitySeedOptions> seedOptions) : ISeedContributor
{
    public int Order => DemoSeed.Order.Roles;

    public SeedEnvironments Environments => SeedEnvironments.Development;

    public async Task SeedAsync(SeedContext context, CancellationToken cancellationToken)
    {
        if (!await DemoSeed.IsSystemTenantAsync(context, tenants, cancellationToken))
        {
            return;
        }

        var tenantId = context.TenantId!.Value;
        var existing = await db.Set<Role>().Where(r => r.TenantId == tenantId).ToDictionaryAsync(r => r.Name!, StringComparer.OrdinalIgnoreCase, cancellationToken);
        foreach (var seed in seedOptions.Value.Roles)
        {
            if (existing.TryGetValue(seed.Name, out var role))
            {
                role.SelectableForRegistration |= seed.SelectableForRegistration;
                continue;
            }

            role = new Role
            {
                Name = seed.Name, NormalizedName = seed.Name.ToUpperInvariant(), Description = seed.Description, TenantId = tenantId,
                SelectableForRegistration = seed.SelectableForRegistration,
            };
            db.Add(role);
            db.AddRange(seed.Permissions.Select(p => new PermissionGrant { TenantId = tenantId, Name = p, ProviderType = PermissionProviderType.Role, ProviderKey = role.Id }));
        }
    }
}
