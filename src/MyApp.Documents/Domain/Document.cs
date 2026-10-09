using Coworkee.Domain;
using Coworkee.Infrastructure.Persistence;
using MyApp.Contracts.Documents;
using Nextended.Core.Facets;

namespace MyApp.Documents.Domain;

[Realtime(DocumentPermissions.Documents.View)]
public sealed class Document : AuditedEntity, IMultiTenant
{
    public required string Title { get; set; }

    public string? Description { get; set; }

    [ProvideFacet(Label = "Visibility")]
    public bool IsPublic { get; set; }

    [ProvideFacet(Label = "Type", ValuePath = "DocumentTypeId", LabelPath = "DocumentType.Name", ValueType = typeof(Guid))]
    public Guid? DocumentTypeId { get; set; }

    public DocumentType? DocumentType { get; set; }

    public Guid? OwnerId { get; set; }

    public required string FileName { get; set; }

    [ProvideFacet(Label = "File type")]
    public required string MimeType { get; set; }

    public long Size { get; set; }

    public required string BlobKey { get; set; }

    public Guid TenantId { get; set; }
}
