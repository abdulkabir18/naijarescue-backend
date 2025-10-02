using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.EntityTypeConfigurations
{
    public class MessageEntityTypeConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Messages");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.CreatedBy).HasMaxLength(100);
            builder.Property(m => m.CreatedAt).IsRequired();
            builder.Property(m => m.UpdatedAt);
            builder.Property(m => m.DeletedAt);
            builder.Property(m => m.IsDeleted).IsRequired();
            builder.Property(m => m.Content).IsRequired(false).HasMaxLength(2000);
            builder.Property(m => m.MessageType).HasConversion<int>().IsRequired();
            builder.Property(m => m.Status).HasConversion<int>().IsRequired();
            builder.Property(m => m.MediaUrl).HasMaxLength(1000);
            builder.Property(m => m.SentAt).IsRequired();

            builder.HasOne(m => m.Chat)
                   .WithMany(c => c.Messages)
                   .HasForeignKey(m => m.ChatId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(m => m.Sender)
                   .WithMany(u => u.Messages)
                   .HasForeignKey(m => m.SenderId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
