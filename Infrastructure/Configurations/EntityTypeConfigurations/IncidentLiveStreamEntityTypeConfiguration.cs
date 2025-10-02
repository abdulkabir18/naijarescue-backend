using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.EntityTypeConfigurations
{
       public class IncidentLiveStreamEntityTypeConfiguration : IEntityTypeConfiguration<IncidentLiveStream>
       {
              public void Configure(EntityTypeBuilder<IncidentLiveStream> builder)
              {
                     builder.ToTable("IncidentLiveStreams");

                     builder.HasKey(ls => ls.Id);

                     builder.Property(ls => ls.CreatedBy).HasMaxLength(100);
                     builder.Property(ls => ls.CreatedAt).IsRequired();
                     builder.Property(ls => ls.UpdatedAt);
                     builder.Property(ls => ls.DeletedAt);
                     builder.Property(ls => ls.IsDeleted).IsRequired();
                     builder.Property(ls => ls.StreamKey).IsRequired().HasMaxLength(100);
                     builder.Property(ls => ls.StartedAt).IsRequired();
                     builder.Property(ls => ls.EndedAt).IsRequired(false);

                     builder.HasIndex(ls => ls.StreamKey).IsUnique();

                     builder.HasOne(ls => ls.Incident)
                            .WithMany(i => i.LiveStreams)
                            .HasForeignKey(ls => ls.IncidentId)
                            .OnDelete(DeleteBehavior.Cascade);

                     builder.HasMany(ls => ls.Participants)
                            .WithOne(p => p.LiveStream)
                            .HasForeignKey(p => p.LiveStreamId)
                            .OnDelete(DeleteBehavior.Cascade);
              }
       }
}
