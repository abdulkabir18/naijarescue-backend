using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.EntityTypeConfigurations
{
       public class AgencySupportedIncidentEntityTypeConfiguration : IEntityTypeConfiguration<AgencySupportedIncident>
       {
              public void Configure(EntityTypeBuilder<AgencySupportedIncident> builder)
              {
                     builder.ToTable("AgencySupportedIncidents");

                     builder.HasKey(asi => asi.Id);

                     builder.Property(asi => asi.Type)
                            .HasConversion<int>()
                            .IsRequired();

                     builder.Property(asi => asi.AgencyId)
                            .IsRequired();

                     builder.HasOne(asi => asi.Agency)
                            .WithMany(a => a.SupportedIncidents)
                            .HasForeignKey(asi => asi.AgencyId)
                            .OnDelete(DeleteBehavior.Cascade);

                     builder.HasIndex(asi => new { asi.AgencyId, asi.Type })
                            .IsUnique();
              }
       }
}
