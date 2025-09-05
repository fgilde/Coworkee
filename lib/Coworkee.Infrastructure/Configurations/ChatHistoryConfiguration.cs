using lib.Coworkee.Application.Common.Models.Chat;
using lib.Coworkee.Infrastructure.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace lib.Coworkee.Infrastructure.Configurations;

public class ChatHistoryConfiguration : IEntityTypeConfiguration<ChatHistory<ApplicationUser>>
{
    public void Configure(EntityTypeBuilder<ChatHistory<ApplicationUser>> builder)
    {
        builder.ToTable("ChatHistory");

        builder.HasOne(d => d.FromUser)
            .WithMany(p => p.ChatHistoryFromUsers)
            .HasForeignKey(d => d.FromUserId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.HasOne(d => d.ToUser)
            .WithMany(p => p.ChatHistoryToUsers)
            .HasForeignKey(d => d.ToUserId)
            .OnDelete(DeleteBehavior.ClientSetNull);
    }
}