using Coworkee.Application.Messaging;
using Coworkee.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MyApp.Contracts;
using MyApp.Contracts.Documents;
using MyApp.Documents.Features.DocumentTypes.Commands.AddEdit;
using MyApp.Documents.Features.DocumentTypes.Commands.Delete;

namespace MyApp.Documents.Endpoints;

internal static class DocumentTypeEndpoints
{
    public static void MapDocumentTypeEndpoints(this IEndpointRouteBuilder app)
    {
        var types = app.MapCoworkeeApi("/api/v1/document-types").WithTags("Document types").RequireAuthorization();
        types.MapPost("/", (AddEditDocumentTypeRequest body, IDispatcher d, CancellationToken ct) => d.SendAsync(new AddEditDocumentTypeCommand(null, body), ct).ToHttpResult());
        types.MapPut("/{id:guid}", (Guid id, AddEditDocumentTypeRequest body, IDispatcher d, CancellationToken ct) => d.SendAsync(new AddEditDocumentTypeCommand(id, body), ct).ToHttpResult());
        types.MapPost("/delete", (IdsRequest body, IDispatcher d, CancellationToken ct) => d.SendAsync(new DeleteDocumentTypesCommand(body.Ids), ct).ToHttpResult());
    }
}
