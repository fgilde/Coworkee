using Coworkee.Infrastructure.Models.Identity;
using Coworkee.Domain.Contracts;
using Coworkee.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Coworkee.Application.Common.Models.Chat;
using Coworkee.Application.Contracts.Services;
using Coworkee.Domain.Entities.ExtendedAttributes;
using Coworkee.Domain.Entities.Identity;
using Coworkee.Domain.Entities.Localization;
using Coworkee.Domain.Entities.Misc;
using Coworkee.Domain.Entities.Notifications;

namespace Coworkee.Infrastructure.Contexts
{


    /*

        dotnet ef migrations add AddRowVersion --project src\Infrastructure --startup-project src\Server --context Coworkee.Infrastructure.Contexts.ApplicationDbContext

    */

    public class ApplicationDbContext : AuditableContext
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeService _dateTimeService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService currentUserService, IDateTimeService dateTimeService)
            : base(options)
        {
            _currentUserService = currentUserService;
            _dateTimeService = dateTimeService;
        }

        public DbSet<ChatHistory<ApplicationUser>> ChatHistories { get; set; }
        public DbSet<UserInformations> UserInformations { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Translation> Translations { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<DocumentExtendedAttribute> DocumentExtendedAttributes { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>().ToList())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedOn = _dateTimeService.NowUtc;
                        entry.Entity.CreatedBy = _currentUserService.UserId;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedOn = _dateTimeService.NowUtc;
                        entry.Entity.LastModifiedBy = _currentUserService.UserId;
                        break;
                }
            }
            if (_currentUserService.UserId == null)
            {
                return await base.SaveChangesAsync(cancellationToken);
            }

            return await base.SaveChangesAsync(_currentUserService.UserId, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                builder.Entity(entityType.ClrType)
                    .Property<byte[]>("RowVersion")
                    .IsRowVersion();

                foreach (var property in entityType.GetProperties()
                             .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
                {
                    property.SetColumnType("decimal(18,2)");
                }

            }

            base.OnModelCreating(builder);
            
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}