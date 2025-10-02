using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.EntityTypeConfigurations
{
    public class IncidentLocationUpdateEntityTypeConfiguration : IEntityTypeConfiguration<IncidentLocationUpdate>
    {
        public void Configure(EntityTypeBuilder<IncidentLocationUpdate> builder)
        {
            builder.ToTable("IncidentLocationUpdates");

            builder.HasKey(lu => lu.Id);

            builder.Property(lu => lu.CreatedBy).HasMaxLength(100);
            builder.Property(lu => lu.CreatedAt).IsRequired();
            builder.Property(lu => lu.UpdatedAt);
            builder.Property(lu => lu.DeletedAt);
            builder.Property(lu => lu.IsDeleted).IsRequired();
            builder.Property(lu => lu.RecordedAt).IsRequired();

            builder.OwnsOne(lu => lu.Location, loc =>
            {
                loc.Property(l => l.Latitude)
                   .HasColumnName("Latitude")
                   .HasPrecision(9, 6);

                loc.Property(l => l.Longitude)
                   .HasColumnName("Longitude")
                   .HasPrecision(9, 6);
            });

            builder.HasOne(lu => lu.Incident)
                   .WithMany(i => i.LocationUpdates)
                   .HasForeignKey(lu => lu.IncidentId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
