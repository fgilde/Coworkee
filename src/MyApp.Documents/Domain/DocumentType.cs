using Coworkee.Domain;
using Coworkee.Infrastructure.Persistence;

namespace MyApp.Documents.Domain;

public sealed class DocumentType : AuditedEntity, IMultiTenant
{
    public required string Name { get; set; }

    public string? Description { get; set; }

    public Guid TenantId { get; set; }
}
