using Coworkee.Contracts.Identity;
using Coworkee.ResponseFilters;
using MyApp.Contracts.Catalog;
using Nextended.ResponseFilters;

namespace MyApp.Catalog.ResponseFilters;

internal sealed class DashboardResponseFilter : ResponseFilter<DashboardDto>
{
    public DashboardResponseFilter()
    {
        Nullify(d => d.Users).UnlessGranted(IdentityPermissions.Users.View);
        Nullify(d => d.Roles).UnlessGranted(IdentityPermissions.Roles.View);
    }
}
