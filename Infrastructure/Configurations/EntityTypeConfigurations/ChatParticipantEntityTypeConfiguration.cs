using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.EntityTypeConfigurations
{
    public class ChatParticipantEntityTypeConfiguration : IEntityTypeConfiguration<ChatParticipant>
    {
        public void Configure(EntityTypeBuilder<ChatParticipant> builder)
        {
            builder.ToTable("ChatParticipants");

            builder.HasKey(cp => cp.Id);

            builder.Property(cp => cp.CreatedBy).HasMaxLength(100);
            builder.Property(cp => cp.CreatedAt).IsRequired();
            builder.Property(cp => cp.UpdatedAt);
            builder.Property(cp => cp.DeletedAt);
            builder.Property(cp => cp.IsDeleted).IsRequired();
            builder.Property(cp => cp.Role).IsRequired().HasMaxLength(50);

            builder.HasOne(cp => cp.Chat)
                   .WithMany(c => c.Participants)
                   .HasForeignKey(cp => cp.ChatId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cp => cp.User)
                   .WithMany(u => u.ChatParticipations)
                   .HasForeignKey(cp => cp.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(cp => new { cp.ChatId, cp.UserId }).IsUnique();
        }
    }
}
