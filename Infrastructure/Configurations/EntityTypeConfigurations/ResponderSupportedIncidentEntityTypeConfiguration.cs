using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.EntityTypeConfigurations
{
       public class ResponderSupportedIncidentEntityTypeConfiguration : IEntityTypeConfiguration<ResponderSupportedIncident>
       {
              public void Configure(EntityTypeBuilder<ResponderSupportedIncident> builder)
              {
                     builder.ToTable("ResponderSupportedIncidents");

                     builder.HasKey(rsi => rsi.Id);

                     builder.Property(rsi => rsi.Type).HasConversion<int>().IsRequired();
                     builder.Property(rsi => rsi.ResponderId).IsRequired();

                     builder.HasOne(rsi => rsi.Responder)
                            .WithMany(r => r.Specialties)
                            .HasForeignKey(rsi => rsi.ResponderId)
                            .OnDelete(DeleteBehavior.Cascade);

                     builder.HasIndex(rsi => new { rsi.ResponderId, rsi.Type }).IsUnique();
              }
       }
}
