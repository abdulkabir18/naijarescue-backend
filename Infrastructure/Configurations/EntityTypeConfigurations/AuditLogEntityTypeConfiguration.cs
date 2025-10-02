using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.EntityTypeConfigurations
{
    public class AuditLogEntityTypeConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Timestamp)
                   .IsRequired();

            builder.Property(a => a.Action)
                   .HasConversion<int>()
                   .IsRequired();

            builder.Property(a => a.EntityName)
                   .HasMaxLength(200);

            builder.Property(a => a.Description)
                   .HasMaxLength(1000);

            builder.Property(a => a.IpAddress)
                   .HasMaxLength(100);

            builder.Property(a => a.UserAgent)
                   .HasMaxLength(300);

            // 🔑 Relation: AuditLog → User (optional)
            builder.HasOne(a => a.User)
                   .WithMany() // no back-collection on User
                   .HasForeignKey(a => a.UserId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
