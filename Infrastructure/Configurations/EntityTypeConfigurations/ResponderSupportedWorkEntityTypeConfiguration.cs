using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.EntityTypeConfigurations
{
       public class ResponderSupportedWorkEntityTypeConfiguration : IEntityTypeConfiguration<ResponderSupportedWork>
       {
              public void Configure(EntityTypeBuilder<ResponderSupportedWork> builder)
              {
                     builder.ToTable("ResponderSupportedWorks");

                     builder.HasKey(rsw => rsw.Id);

                     builder.Property(rsw => rsw.Type).HasConversion<int>().IsRequired();
                     builder.Property(rsw => rsw.ResponderId).IsRequired();

                     builder.HasOne(rsw => rsw.Responder)
                            .WithMany(r => r.Capabilities)
                            .HasForeignKey(rsw => rsw.ResponderId)
                            .OnDelete(DeleteBehavior.Cascade);

                     builder.HasIndex(rsw => new { rsw.ResponderId, rsw.Type }).IsUnique();
              }
       }
}
