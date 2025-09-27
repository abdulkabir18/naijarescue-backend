using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.EntityTypeConfigurations
{
    public class AgencyEntityTypeConfiguration : IEntityTypeConfiguration<Agency>
    {
        public void Configure(EntityTypeBuilder<Agency> builder)
        {
            builder.ToTable("Agencies");

            builder.HasKey(a => a.Id);
            builder.Property(a => a.CreatedBy).HasMaxLength(100);
            builder.Property(a => a.CreatedAt).IsRequired();
            builder.Property(a => a.UpdatedAt);
            builder.Property(a => a.DeletedAt);
            builder.Property(a => a.IsDeleted).IsRequired();
            builder.Property(a => a.Name).IsRequired().HasMaxLength(200);
            builder.Property(a => a.LogoUrl).HasMaxLength(500);
            builder.Property(a => a.IsActive).IsRequired();

            builder.OwnsOne(a => a.Email, email =>
            {
                email.Property(e => e.Value).HasColumnName("Email").IsRequired().HasMaxLength(255);
            });

            builder.OwnsOne(a => a.PhoneNumber, phone =>
            {
                phone.Property(p => p.Value).HasColumnName("PhoneNumber").IsRequired().HasMaxLength(18);
            });

            builder.OwnsOne(a => a.Address, address =>
            {
                address.Property(ad => ad.Street).HasMaxLength(200);
                address.Property(ad => ad.City).HasMaxLength(100);
                address.Property(ad => ad.State).HasMaxLength(100);
                address.Property(ad => ad.PostalCode).HasMaxLength(20);
                address.Property(ad => ad.Country).HasMaxLength(100);
            });

            builder.HasMany(a => a.Responders)
                .WithOne(r => r.Agency)
                .HasForeignKey(u => u.AgencyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.AgencyAdmin)
                .WithOne(a => a.Agency)
                .HasForeignKey<Agency>(a => a.AgencyAdminId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.SupportedIncidents)
                .WithOne()
                .HasForeignKey(si => si.AgencyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(a => a.SupportedWorkTypes)
                .WithOne()
                .HasForeignKey(sw => sw.AgencyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
