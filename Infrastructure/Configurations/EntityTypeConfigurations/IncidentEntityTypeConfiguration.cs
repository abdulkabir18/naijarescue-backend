using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.EntityTypeConfigurations
{
    public class IncidentEntityTypeConfiguration : IEntityTypeConfiguration<Incident>
    {
        public void Configure(EntityTypeBuilder<Incident> builder)
        {
            builder.ToTable("Incidents");

            builder.HasKey(a => a.Id);
            builder.Property(a => a.CreatedBy).HasMaxLength(100);
            builder.Property(a => a.CreatedAt).IsRequired();
            builder.Property(a => a.UpdatedAt);
            builder.Property(a => a.DeletedAt);
            builder.Property(a => a.IsDeleted).IsRequired();
            builder.Property(i => i.Type).HasConversion<int>().IsRequired();
            builder.Property(i => i.Status).HasConversion<int>().IsRequired();
            builder.Property(i => i.OccurredAt).IsRequired();
            builder.Property(i => i.IsAnonymous).IsRequired();
            builder.Property(i => i.ReferenceCode).IsRequired().HasMaxLength(50);
            builder.HasIndex(i => i.ReferenceCode).IsUnique();
            builder.Property(i => i.ReporterName).HasMaxLength(150);

            builder.OwnsOne(i => i.Location, location =>
            {
                location.Property(l => l.Latitude)
                        .HasColumnName("Latitude")
                        .HasPrecision(9, 6);

                location.Property(l => l.Longitude)
                        .HasColumnName("Longitude")
                        .HasPrecision(9, 6);
            });

            builder.OwnsOne(a => a.Address, address =>
            {
                address.Property(ad => ad.Street).HasMaxLength(200);
                address.Property(ad => ad.City).HasMaxLength(100);
                address.Property(ad => ad.State).HasMaxLength(100);
                address.Property(ad => ad.PostalCode).HasMaxLength(20);
                address.Property(ad => ad.Country).HasMaxLength(100);
            });

            builder.OwnsOne(a => a.ReporterEmail, email =>
            {
                email.Property(e => e.Value)
                     .HasColumnName("ReporterEmail")
                     .IsRequired(false)
                     .HasMaxLength(255);
            });

            builder.OwnsOne(a => a.ReporterPhoneNumber, phone =>
            {
                phone.Property(p => p.Value)
                     .HasColumnName("ReporterPhoneNumber")
                     .IsRequired(false)
                     .HasMaxLength(18);
            });

            builder.HasOne(i => i.User)
                   .WithMany(u => u.Incidents)
                   .HasForeignKey(i => i.UserId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(i => i.AssignedResponders)
                   .WithOne(ar => ar.Incident)
                   .HasForeignKey(ar => ar.IncidentId);

            builder.HasMany(i => i.IncidentMedias)
                   .WithOne(m => m.Incident)
                   .HasForeignKey(m => m.IncidentId);

            builder.HasMany(i => i.LiveStreams)
                   .WithOne(ls => ls.Incident)
                   .HasForeignKey(ls => ls.IncidentId);
        }
    }
}
