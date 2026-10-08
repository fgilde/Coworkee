using Coworkee.Application.Messaging;
using Coworkee.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyApp.Contracts;
using MyApp.Contracts.Documents;
using MyApp.Documents.Features.Documents.Commands.Delete;
using MyApp.Documents.Features.Documents.Commands.Update;
using MyApp.Documents.Features.Documents.Commands.Upload;
using MyApp.Documents.Features.Documents.Queries.GetById;
using MyApp.Documents.Features.Documents.Queries.Open;

namespace MyApp.Documents.Endpoints;

internal static class DocumentEndpoints
{
    private const string SandboxPolicy = "sandbox; default-src 'none'; img-src 'self' data:; media-src 'self'; style-src 'unsafe-inline'";

    public static void MapDocumentEndpoints(this IEndpointRouteBuilder app)
    {
        var documents = app.MapCoworkeeApi("/api/v1/documents").WithTags("Documents").RequireAuthorization();
        documents.MapGet("/{id:guid}", (Guid id, IDispatcher d, CancellationToken ct) => d.SendAsync(new GetDocumentByIdQuery(id), ct).ToHttpResult());
        documents.MapPost("/", UploadAsync).DisableAntiforgery().WithMetadata(new RequestSizeLimitAttribute(MyAppDocumentsModule.MaxSize + 1024 * 1024));
        documents.MapPut("/{id:guid}", (Guid id, UpdateDocumentRequest body, IDispatcher d, CancellationToken ct) => d.SendAsync(new UpdateDocumentCommand(id, body), ct).ToHttpResult());
        documents.MapPost("/delete", (IdsRequest body, IDispatcher d, CancellationToken ct) => d.SendAsync(new DeleteDocumentsCommand(body.Ids), ct).ToHttpResult());
        documents.MapGet("/{id:guid}/content", ContentAsync);
    }

    private static async Task<IResult> UploadAsync(IFormFile file, [FromForm] string title, [FromForm] string? description, [FromForm] bool? isPublic,
        [FromForm] Guid? documentTypeId, IDispatcher dispatcher, CancellationToken cancellationToken)
    {
        var request = new UpdateDocumentRequest { Title = title, Description = description, IsPublic = isPublic ?? false, DocumentTypeId = documentTypeId };
        await using var content = file.OpenReadStream();
        return await dispatcher.SendAsync(new UploadDocumentCommand(request, file.FileName, file.Length, content), cancellationToken).ToHttpResult();
    }

    private static async Task<IResult> ContentAsync(Guid id, bool? download, HttpResponse response, IDispatcher dispatcher, CancellationToken cancellationToken)
    {
        var result = await dispatcher.SendAsync(new OpenDocumentQuery(id), cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error!.ToProblem();
        }

        var document = result.Value;
        var inline = download != true;
        response.Headers.XContentTypeOptions = "nosniff";
        response.Headers.CacheControl = "private, no-store";
        if (inline)
        {
            response.Headers.ContentSecurityPolicy = SandboxPolicy;
        }

        return Results.File(document.Content, document.MimeType, inline ? null : document.FileName, enableRangeProcessing: true);
    }
}
