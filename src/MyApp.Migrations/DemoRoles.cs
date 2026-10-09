using Coworkee.Contracts.Identity;
using Coworkee.Core.Security;
using Coworkee.Identity.Domain;
using Coworkee.Identity.Setup;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyApp.Infrastructure;

namespace MyApp.Migrations;

/// <summary>
/// The identity seed runs only on a fresh database; this keeps its roles on every run: a missing role comes with its permissions, an
/// existing one only gets the registration flag. Users, passwords and the demo data stay untouched.
/// </summary>
internal static class DemoRoles
{
    public static async Task EnsureAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var provider = scope.ServiceProvider;
        if (await provider.GetRequiredService<ITenantDirectory>().GetSystemTenantIdAsync(cancellationToken) is not { } tenantId)
        {
            return;
        }

        using var actor = CurrentUserScope.Begin(new ImpersonatedUser(null, tenantId));
        var db = provider.GetRequiredService<MyAppDbContext>();
        var existing = await db.Set<Role>().Where(r => r.TenantId == tenantId).ToDictionaryAsync(r => r.Name!, StringComparer.OrdinalIgnoreCase, cancellationToken);
        foreach (var seed in provider.GetRequiredService<IOptions<IdentitySeedOptions>>().Value.Roles)
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

        await db.SaveChangesAsync(cancellationToken);
    }
}
