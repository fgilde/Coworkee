using Coworkee.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MyApp.Sales.Domain;

namespace MyApp.Sales.Persistence;

internal sealed class SalesModelContributor : IModelContributor
{
    public void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(customer =>
        {
            customer.ToTable("Customers", "app");
            customer.Property(c => c.Name).HasMaxLength(200);
            customer.Property(c => c.City).HasMaxLength(100);
        });

        modelBuilder.Entity<SalesOrder>(order =>
        {
            order.ToTable("SalesOrders", "app");
            order.Property(o => o.Number).HasMaxLength(20);
            order.Property(o => o.Region).HasMaxLength(50);
            order.Property(o => o.Total).HasPrecision(18, 2);
            order.HasOne(o => o.Customer).WithMany().HasForeignKey(o => o.CustomerId).OnDelete(DeleteBehavior.Restrict);
            order.HasIndex(o => new { o.TenantId, o.Number }).IsUnique();
            order.HasIndex(o => o.CustomerId);
        });
    }
}
