using Coworkee.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MyApp.Documents.Domain;

namespace MyApp.Documents.Persistence;

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
            document.HasOne(d => d.DocumentType).WithMany().HasForeignKey(d => d.DocumentTypeId).OnDelete(DeleteBehavior.SetNull);
            document.HasIndex(d => d.OwnerId);
        });
    }
}
