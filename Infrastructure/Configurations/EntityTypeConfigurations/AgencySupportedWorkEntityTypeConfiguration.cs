using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.EntityTypeConfigurations
{
       public class AgencySupportedWorkEntityTypeConfiguration : IEntityTypeConfiguration<AgencySupportedWork>
       {
              public void Configure(EntityTypeBuilder<AgencySupportedWork> builder)
              {
                     builder.ToTable("AgencySupportedWorks");

                     builder.HasKey(asw => asw.Id);

                     builder.Property(asw => asw.Type)
                            .HasConversion<int>()
                            .IsRequired();

                     builder.Property(asw => asw.AgencyId)
                            .IsRequired();

                     builder.HasOne(asw => asw.Agency)
                            .WithMany(a => a.SupportedWorkTypes)
                            .HasForeignKey(asw => asw.AgencyId)
                            .OnDelete(DeleteBehavior.Cascade);

                     builder.HasIndex(asw => new { asw.AgencyId, asw.Type })
                            .IsUnique();
              }
       }
}
