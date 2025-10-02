using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.EntityTypeConfigurations
{
    public class ChatEntityTypeConfiguration : IEntityTypeConfiguration<Chat>
    {
        public void Configure(EntityTypeBuilder<Chat> builder)
        {
            builder.ToTable("Chats");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.CreatedBy).HasMaxLength(100);
            builder.Property(c => c.CreatedAt).IsRequired();
            builder.Property(c => c.UpdatedAt);
            builder.Property(c => c.DeletedAt);
            builder.Property(c => c.IsDeleted).IsRequired();
            builder.Property(c => c.ChatType).HasConversion<int>().IsRequired();
            builder.Property(c => c.IsActive).IsRequired();

            builder.HasOne(c => c.Incident)
                .WithOne(i => i.Chat)
                .HasForeignKey<Chat>(c => c.IncidentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Participants)
                   .WithOne(p => p.Chat)
                   .HasForeignKey(p => p.ChatId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Messages)
                   .WithOne(m => m.Chat)
                   .HasForeignKey(m => m.ChatId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
