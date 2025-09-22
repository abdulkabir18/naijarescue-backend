using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.EntityTypeConfigurations
{
    public class ResponderEntityTypeConfiguration : IEntityTypeConfiguration<Responder>
    {
        public void Configure(EntityTypeBuilder<Responder> builder)
        {
            builder.ToTable("Responders");

            builder.HasKey(a => a.Id);
            builder.Property(a => a.CreatedBy).HasMaxLength(100);
            builder.Property(a => a.CreatedAt).IsRequired();
            builder.Property(a => a.UpdatedAt);
            builder.Property(a => a.DeletedAt);
            builder.Property(a => a.IsDeleted).IsRequired();
            builder.Property(r => r.BadgeNumber).HasMaxLength(50);
            builder.Property(r => r.Rank).HasMaxLength(50);
            builder.Property(r => r.Status).HasConversion<int>().IsRequired();
            builder.Property(r => r.IsVerified).IsRequired();

            builder.OwnsOne(r => r.AssignedLocation, loc =>
            {
                loc.Property(l => l.Latitude)
                   .HasColumnName("AssignedLatitude")
                   .HasPrecision(9, 6);

                loc.Property(l => l.Longitude)
                   .HasColumnName("AssignedLongitude")
                   .HasPrecision(9, 6);
            });

            builder.HasOne(r => r.User)
                   .WithOne(u => u.Responder)   
                   .HasForeignKey<Responder>(r => r.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.Capabilities)
                   .WithOne(c => c.Responder)
                   .HasForeignKey(c => c.ResponderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.Specialties)
                   .WithOne(s => s.Responder)
                   .HasForeignKey(s => s.ResponderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.AssignedIncidents)
                   .WithOne(i => i.AssignedResponder) 
                   .HasForeignKey(i => i.AssignedResponderId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
