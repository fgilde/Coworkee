using Coworkee.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MyApp.Catalog.Domain;

namespace MyApp.Catalog.Persistence;

internal sealed class CatalogModelContributor : IModelContributor
{
    public void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brand>(brand =>
        {
            brand.ToTable("Brands", "app");
            brand.Property(b => b.Name).HasMaxLength(200);
            brand.Property(b => b.Description).HasMaxLength(2000);
            brand.Property(b => b.Tax).HasPrecision(9, 4);
            brand.HasIndex(b => new { b.TenantId, b.Name }).IsUnique();
        });

        modelBuilder.Entity<Product>(product =>
        {
            product.ToTable("Products", "app");
            product.Property(p => p.Name).HasMaxLength(200);
            product.Property(p => p.Barcode).HasMaxLength(100);
            product.Property(p => p.Description).HasMaxLength(2000);
            product.Property(p => p.Rate).HasPrecision(18, 4);
            product.HasOne(p => p.Brand).WithMany().HasForeignKey(p => p.BrandId).OnDelete(DeleteBehavior.Restrict);
            product.HasIndex(p => p.BrandId);
        });
    }
}
