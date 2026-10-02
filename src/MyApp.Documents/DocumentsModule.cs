using Coworkee.Application.Authorization;
using Coworkee.Application.Messaging;
using Coworkee.AspNetCore;
using Coworkee.AspNetCore.Http;
using Coworkee.Contracts;
using Coworkee.Core.Modularity;
using Coworkee.Core.Results;
using Coworkee.Core.Security;
using Coworkee.Domain;
using Coworkee.Infrastructure.Persistence;
using Coworkee.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Contracts.Documents;

namespace MyApp.Documents;

/// <summary>Example of files on Coworkee.Storage: typed documents, public or private, shown in MudExFileDisplay when their type is safe.</summary>
[DependsOn(typeof(CoworkeeStorageModule))]
public sealed class MyAppDocumentsModule : CoworkeeModule, IWebModule
{
    public const long MaxSize = 100L * 1024 * 1024;

    public override void ConfigureServices(ModuleServiceContext context)
    {
        context.Services.AddMessagingFromAssembly(typeof(MyAppDocumentsModule).Assembly);
        context.Services.AddSingleton<IModelContributor, DocumentModelContributor>();
        context.Services.AddSingleton<IPermissionDefinitionContributor, DocumentPermissionDefinitions>();
    }

    public void ConfigureApplication(WebApplication app)
    {
        var types = app.MapGroup("/api/v1/document-types").WithTags("Documents").RequireAuthorization();
        types.MapGet("/", (IDispatcher d, CancellationToken ct) => d.SendAsync(new GetDocumentTypes(), ct).ToHttpResult());
        types.MapPost("/", (DocumentTypeRequest body, IDispatcher d, CancellationToken ct) => d.SendAsync(new SaveDocumentType(null, body), ct).ToHttpResult());
        types.MapPut("/{id:guid}", (Guid id, DocumentTypeRequest body, IDispatcher d, CancellationToken ct) => d.SendAsync(new SaveDocumentType(id, body), ct).ToHttpResult());
        types.MapDelete("/{id:guid}", (Guid id, IDispatcher d, CancellationToken ct) => d.SendAsync(new DeleteDocumentType(id), ct).ToHttpResult());

        var documents = app.MapGroup("/api/v1/documents").WithTags("Documents").RequireAuthorization();
        documents.MapGet("/", (int? page, int? pageSize, string? search, IDispatcher d, CancellationToken ct) =>
            d.SendAsync(new GetDocuments(new PageRequest(page ?? 1, Math.Clamp(pageSize ?? 25, 1, 200), search)), ct).ToHttpResult());
        documents.MapGet("/{id:guid}", (Guid id, IDispatcher d, CancellationToken ct) => d.SendAsync(new GetDocument(id), ct).ToHttpResult());

        // the api is called with a bearer token through the BFF (which checks its own CSRF header), so form antiforgery does not apply
        documents.MapPost("/", (IFormFile file, [Microsoft.AspNetCore.Mvc.FromForm] string title, [Microsoft.AspNetCore.Mvc.FromForm] string? description,
            [Microsoft.AspNetCore.Mvc.FromForm] bool? isPublic, [Microsoft.AspNetCore.Mvc.FromForm] Guid? documentTypeId, IDispatcher d, CancellationToken ct) =>
            d.SendAsync(new UploadDocument(new DocumentRequest(title, description, isPublic ?? false, documentTypeId), file.FileName, file.Length, file.OpenReadStream()), ct).ToHttpResult()).DisableAntiforgery().WithMetadata(new Microsoft.AspNetCore.Mvc.RequestSizeLimitAttribute(MaxSize + 1024 * 1024));
        documents.MapPut("/{id:guid}", (Guid id, DocumentRequest body, IDispatcher d, CancellationToken ct) => d.SendAsync(new UpdateDocument(id, body), ct).ToHttpResult());
        documents.MapDelete("/{id:guid}", (Guid id, IDispatcher d, CancellationToken ct) => d.SendAsync(new DeleteDocument(id), ct).ToHttpResult());
        documents.MapGet("/{id:guid}/content", async (Guid id, bool? download, HttpResponse response, IDispatcher d, CancellationToken ct) =>
        {
            var result = await d.SendAsync(new OpenDocument(id), ct);
            if (!result.IsSuccess)
            {
                return result.Error!.ToProblem();
            }

            var (content, fileName, mimeType) = result.Value;
            var inline = download != true && DocumentInlineTypes.All.Contains(mimeType);
            response.Headers.XContentTypeOptions = "nosniff";
            response.Headers.CacheControl = "private, no-store";
            if (inline)
            {
                // shown in the app's origin: nothing in it may run
                response.Headers.ContentSecurityPolicy = "sandbox; default-src 'none'; img-src 'self' data:; media-src 'self'; style-src 'unsafe-inline'";
            }

            return Results.File(content, mimeType, inline ? null : fileName, enableRangeProcessing: true);
        });
    }
}

public sealed class DocumentType : AuditedEntity, IMultiTenant
{
    public required string Name { get; set; }

    public string? Description { get; set; }

    public Guid TenantId { get; set; }
}

[Realtime(DocumentPermissions.View)]
public sealed class Document : AuditedEntity, IMultiTenant
{
    public required string Title { get; set; }

    public string? Description { get; set; }

    public bool IsPublic { get; set; }

    public Guid? DocumentTypeId { get; set; }

    public Guid? OwnerId { get; set; }

    public required string FileName { get; set; }

    public required string MimeType { get; set; }

    public long Size { get; set; }

    public required string BlobKey { get; set; }

    public Guid TenantId { get; set; }
}

internal sealed class DocumentModelContributor : IModelContributor
{
    public void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DocumentType>(type =>
        {
            type.ToTable("DocumentTypes", "app");
            type.Property(t => t.Name).HasMaxLength(200);
            type.Property(t => t.Description).HasMaxLength(2000);
            type.HasIndex(t => new { t.TenantId, t.Name }).IsUnique();
        });

        modelBuilder.Entity<Document>(document =>
        {
            document.ToTable("Documents", "app");
            document.Property(d => d.Title).HasMaxLength(300);
            document.Property(d => d.Description).HasMaxLength(4000);
            document.Property(d => d.FileName).HasMaxLength(255);
            document.Property(d => d.MimeType).HasMaxLength(100);
            document.Property(d => d.BlobKey).HasMaxLength(512);
            document.HasOne<DocumentType>().WithMany().HasForeignKey(d => d.DocumentTypeId).OnDelete(DeleteBehavior.SetNull);
            document.HasIndex(d => d.OwnerId);
        });
    }
}

internal sealed class DocumentPermissionDefinitions : IPermissionDefinitionContributor
{
    public void Define(PermissionDefinitionContext context) =>
        context.Group(DocumentPermissions.GroupName, "Documents")
            .Add(DocumentPermissions.View, "View public and own documents")
            .Add(DocumentPermissions.Upload, "Upload documents", DocumentPermissions.View)
            .Add(DocumentPermissions.Manage, "Edit and delete all documents, manage document types", DocumentPermissions.Upload);
}

[RequiresPermission(DocumentPermissions.View)]
public sealed record GetDocumentTypes : IQuery<Result<IReadOnlyList<DocumentTypeDto>>>;

[RequiresPermission(DocumentPermissions.Manage)]
public sealed record SaveDocumentType(Guid? Id, DocumentTypeRequest Request) : ICommand<Result<DocumentTypeDto>>;

[RequiresPermission(DocumentPermissions.Manage)]
public sealed record DeleteDocumentType(Guid Id) : ICommand<Result>;

[RequiresPermission(DocumentPermissions.View)]
public sealed record GetDocuments(PageRequest Page) : IQuery<Result<PagedResult<DocumentDto>>>;

[RequiresPermission(DocumentPermissions.View)]
public sealed record GetDocument(Guid Id) : IQuery<Result<DocumentDto>>;

[RequiresPermission(DocumentPermissions.View)]
public sealed record OpenDocument(Guid Id) : IQuery<Result<(Stream Content, string FileName, string MimeType)>>;

[RequiresPermission(DocumentPermissions.Upload)]
public sealed record UploadDocument(DocumentRequest Request, string FileName, long Size, Stream Content) : ICommand<Result<DocumentDto>>;

[RequiresPermission(DocumentPermissions.View)]
public sealed record UpdateDocument(Guid Id, DocumentRequest Request) : ICommand<Result<DocumentDto>>;

[RequiresPermission(DocumentPermissions.View)]
public sealed record DeleteDocument(Guid Id) : ICommand<Result>;

internal sealed class DocumentHandlers(CoworkeeDbContext db, ICurrentUser currentUser, IPermissionChecker permissions, IBlobStorage storage, TimeProvider clock)
    : IHandler<GetDocumentTypes, Result<IReadOnlyList<DocumentTypeDto>>>,
      IHandler<SaveDocumentType, Result<DocumentTypeDto>>,
      IHandler<DeleteDocumentType, Result>,
      IHandler<GetDocuments, Result<PagedResult<DocumentDto>>>,
      IHandler<GetDocument, Result<DocumentDto>>,
      IHandler<OpenDocument, Result<(Stream Content, string FileName, string MimeType)>>,
      IHandler<UploadDocument, Result<DocumentDto>>,
      IHandler<UpdateDocument, Result<DocumentDto>>,
      IHandler<DeleteDocument, Result>
{
    private static readonly Error NotFound = Error.NotFound("documents.not_found", "The document does not exist.");
    private static readonly FileExtensionContentTypeProvider ContentTypes = new();

    // executables and scripts are refused whatever they claim to be
    private static readonly HashSet<string> Forbidden = [".exe", ".dll", ".bat", ".cmd", ".com", ".msi", ".scr", ".ps1", ".vbs", ".js", ".jar", ".sh", ".hta", ".lnk"];

    public async Task<Result<IReadOnlyList<DocumentTypeDto>>> HandleAsync(GetDocumentTypes query, CancellationToken cancellationToken)
    {
        IReadOnlyList<DocumentTypeDto> types = await db.Set<DocumentType>().AsNoTracking().OrderBy(t => t.Name).Select(t => new DocumentTypeDto(t.Id, t.Name, t.Description)).ToListAsync(cancellationToken);
        return Result<IReadOnlyList<DocumentTypeDto>>.Success(types);
    }

    public async Task<Result<DocumentTypeDto>> HandleAsync(SaveDocumentType command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Request.Name) || command.Request.Name.Length > 200)
        {
            return Error.Validation("Name", "A name of up to 200 characters is required.");
        }

        var name = command.Request.Name.Trim();
        if (await db.Set<DocumentType>().AnyAsync(t => t.Name == name && t.Id != command.Id, cancellationToken))
        {
            return Error.Conflict("documents.type_exists", "A document type with this name exists.");
        }

        DocumentType type;
        if (command.Id is { } id)
        {
            if (await db.Set<DocumentType>().SingleOrDefaultAsync(t => t.Id == id, cancellationToken) is not { } existing)
            {
                return Error.NotFound("documents.type_not_found", "The document type does not exist.");
            }

            type = existing;
        }
        else
        {
            type = new DocumentType { Name = name };
            db.Add(type);
        }

        type.Name = name;
        type.Description = string.IsNullOrWhiteSpace(command.Request.Description) ? null : command.Request.Description.Trim();
        return new DocumentTypeDto(type.Id, type.Name, type.Description);
    }

    public async Task<Result> HandleAsync(DeleteDocumentType command, CancellationToken cancellationToken)
    {
        if (await db.Set<DocumentType>().SingleOrDefaultAsync(t => t.Id == command.Id, cancellationToken) is not { } type)
        {
            return Error.NotFound("documents.type_not_found", "The document type does not exist.");
        }

        db.Remove(type);
        return Result.Success();
    }

    public async Task<Result<PagedResult<DocumentDto>>> HandleAsync(GetDocuments query, CancellationToken cancellationToken)
    {
        var manager = await permissions.IsGrantedAsync(DocumentPermissions.Manage, cancellationToken);
        var visible = Visible(manager);
        if (query.Page.Search is { Length: > 0 } search)
        {
            var pattern = "%" + search.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_") + "%";
            visible = visible.Where(d => EF.Functions.ILike(d.Title, pattern) || EF.Functions.ILike(d.FileName, pattern));
        }

        var total = await visible.CountAsync(cancellationToken);
        var page = await visible.OrderByDescending(d => d.CreatedAt).Skip((query.Page.Page - 1) * query.Page.PageSize).Take(query.Page.PageSize).ToListAsync(cancellationToken);
        var typeNames = await TypeNamesAsync(page.Select(d => d.DocumentTypeId), cancellationToken);
        return new PagedResult<DocumentDto>(page.Select(d => Map(d, typeNames, manager)).ToList(), total, query.Page.Page, query.Page.PageSize);
    }

    public async Task<Result<DocumentDto>> HandleAsync(GetDocument query, CancellationToken cancellationToken)
    {
        var manager = await permissions.IsGrantedAsync(DocumentPermissions.Manage, cancellationToken);
        return await Visible(manager).SingleOrDefaultAsync(d => d.Id == query.Id, cancellationToken) is { } document
            ? Map(document, await TypeNamesAsync([document.DocumentTypeId], cancellationToken), manager)
            : NotFound;
    }

    public async Task<Result<(Stream Content, string FileName, string MimeType)>> HandleAsync(OpenDocument query, CancellationToken cancellationToken)
    {
        var manager = await permissions.IsGrantedAsync(DocumentPermissions.Manage, cancellationToken);
        if (await Visible(manager).SingleOrDefaultAsync(d => d.Id == query.Id, cancellationToken) is not { } document
            || await storage.OpenReadAsync(document.BlobKey, cancellationToken) is not { } content)
        {
            return NotFound;
        }

        return (content, document.FileName, document.MimeType);
    }

    public async Task<Result<DocumentDto>> HandleAsync(UploadDocument command, CancellationToken cancellationToken)
    {
        var fileName = Path.GetFileName((command.FileName ?? string.Empty).Replace('\\', '/')).Trim();
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (fileName.Length is 0 or > 255 || fileName.Any(char.IsControl))
        {
            return Error.Validation("File", "A file name of up to 255 characters is required.");
        }

        if (Forbidden.Contains(extension))
        {
            return Error.Validation("File", $"Files of type '{extension}' cannot be uploaded.");
        }

        if (command.Size is <= 0 or > MyAppDocumentsModule.MaxSize)
        {
            return Error.Validation("File", $"Files from 1 byte to {MyAppDocumentsModule.MaxSize / (1024 * 1024)} MB can be uploaded.");
        }

        if (await ValidateAsync(command.Request, cancellationToken) is { } problem)
        {
            return problem;
        }

        var key = BlobKeys.New(currentUser.TenantId ?? Guid.Empty, clock);
        var mimeType = ContentTypes.TryGetContentType(fileName, out var type) ? type : "application/octet-stream";
        await storage.PutAsync(key, command.Content, mimeType, cancellationToken);
        var document = new Document
        {
            Title = command.Request.Title.Trim(),
            FileName = fileName,
            MimeType = mimeType,
            Size = command.Size,
            BlobKey = key,
            OwnerId = currentUser.UserId,
        };
        Apply(document, command.Request);
        db.Add(document);
        return Map(document, await TypeNamesAsync([document.DocumentTypeId], cancellationToken), canEdit: true);
    }

    public async Task<Result<DocumentDto>> HandleAsync(UpdateDocument command, CancellationToken cancellationToken)
    {
        if (await EditableAsync(command.Id, cancellationToken) is not { } document)
        {
            return NotFound;
        }

        if (await ValidateAsync(command.Request, cancellationToken) is { } problem)
        {
            return problem;
        }

        Apply(document, command.Request);
        return Map(document, await TypeNamesAsync([document.DocumentTypeId], cancellationToken), canEdit: true);
    }

    public async Task<Result> HandleAsync(DeleteDocument command, CancellationToken cancellationToken)
    {
        if (await EditableAsync(command.Id, cancellationToken) is not { } document)
        {
            return NotFound;
        }

        // ponytail: the file goes before the row is saved; a failed save leaves a row without file (the download then says not found)
        await storage.DeleteAsync(document.BlobKey, cancellationToken);
        db.Remove(document);
        return Result.Success();
    }

    private IQueryable<Document> Visible(bool manager)
    {
        var userId = currentUser.UserId;
        return manager ? db.Set<Document>() : db.Set<Document>().Where(d => d.IsPublic || d.OwnerId == userId);
    }

    /// <summary>Own documents, or every one for managers; others are reported as missing.</summary>
    private async Task<Document?> EditableAsync(Guid id, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;
        var manager = await permissions.IsGrantedAsync(DocumentPermissions.Manage, cancellationToken);
        return await db.Set<Document>().SingleOrDefaultAsync(d => d.Id == id && (manager || d.OwnerId == userId), cancellationToken);
    }

    private async Task<Error?> ValidateAsync(DocumentRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || request.Title.Length > 300)
        {
            return Error.Validation("Title", "A title of up to 300 characters is required.");
        }

        if (request.Description is { Length: > 4000 })
        {
            return Error.Validation("Description", "Up to 4000 characters.");
        }

        return request.DocumentTypeId is { } typeId && !await db.Set<DocumentType>().AnyAsync(t => t.Id == typeId, cancellationToken)
            ? Error.Validation("DocumentTypeId", "Choose an existing document type.")
            : null;
    }

    private static void Apply(Document document, DocumentRequest request)
    {
        document.Title = request.Title.Trim();
        document.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        document.IsPublic = request.IsPublic;
        document.DocumentTypeId = request.DocumentTypeId;
    }

    private async Task<Dictionary<Guid, string>> TypeNamesAsync(IEnumerable<Guid?> ids, CancellationToken cancellationToken)
    {
        var wanted = ids.OfType<Guid>().Distinct().ToList();
        return wanted.Count == 0 ? [] : await db.Set<DocumentType>().AsNoTracking().Where(t => wanted.Contains(t.Id)).ToDictionaryAsync(t => t.Id, t => t.Name, cancellationToken);
    }

    private DocumentDto Map(Document d, Dictionary<Guid, string> typeNames, bool canEdit) => new(
        d.Id, d.Title, d.Description, d.IsPublic, d.DocumentTypeId, d.DocumentTypeId is { } typeId && typeNames.TryGetValue(typeId, out var name) ? name : null,
        d.FileName, d.MimeType, d.Size, d.OwnerId, d.CreatedAt, canEdit || d.OwnerId == currentUser.UserId);
}
