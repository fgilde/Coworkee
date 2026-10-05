using Coworkee.Application.Messaging;
using Coworkee.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MyApp.Catalog.Features.Dashboard.Queries;

namespace MyApp.Catalog.Endpoints;

internal static class DashboardEndpoints
{
    public static void MapDashboardEndpoints(this IEndpointRouteBuilder app) =>
        app.MapCoworkeeApi("/api/v1/dashboard").WithTags("Dashboard").RequireAuthorization()
            .MapGet("/", (IDispatcher d, CancellationToken ct) => d.SendAsync(new GetDashboardQuery(), ct).ToHttpResult());
}
