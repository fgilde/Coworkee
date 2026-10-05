using Coworkee.Application.Messaging;
using Coworkee.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MyApp.Catalog.Features.Products.Commands.AddEdit;
using MyApp.Catalog.Features.Products.Commands.Delete;
using MyApp.Catalog.Features.Products.Queries.GetById;
using MyApp.Contracts;
using MyApp.Contracts.Catalog;

namespace MyApp.Catalog.Endpoints;

internal static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var products = app.MapCoworkeeApi("/api/v1/products").WithTags("Products").RequireAuthorization();
        products.MapGet("/{id:guid}", (Guid id, IDispatcher d, CancellationToken ct) => d.SendAsync(new GetProductByIdQuery(id), ct).ToHttpResult());
        products.MapPost("/", (AddEditProductRequest body, IDispatcher d, CancellationToken ct) => d.SendAsync(new AddEditProductCommand(null, body), ct).ToHttpResult());
        products.MapPut("/{id:guid}", (Guid id, AddEditProductRequest body, IDispatcher d, CancellationToken ct) => d.SendAsync(new AddEditProductCommand(id, body), ct).ToHttpResult());
        products.MapPost("/delete", (IdsRequest body, IDispatcher d, CancellationToken ct) => d.SendAsync(new DeleteProductsCommand(body.Ids), ct).ToHttpResult());
    }
}
