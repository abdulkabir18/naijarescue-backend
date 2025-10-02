using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.EntityTypeConfigurations
{
       public class IncidentLiveStreamParticipantConfiguration : IEntityTypeConfiguration<IncidentLiveStreamParticipant>
       {
              public void Configure(EntityTypeBuilder<IncidentLiveStreamParticipant> builder)
              {
                     builder.ToTable("IncidentLiveStreamParticipants");

                     builder.HasKey(p => p.Id);

                     builder.Property(p => p.JoinedAt).IsRequired();
                     builder.Property(p => p.RejoinedAt).IsRequired(false);
                     builder.Property(p => p.LeftAt).IsRequired(false);

                     builder.HasOne(p => p.LiveStream)
                            .WithMany(ls => ls.Participants)
                            .HasForeignKey(p => p.LiveStreamId)
                            .OnDelete(DeleteBehavior.Cascade);

                     builder.HasOne(p => p.User)
                            .WithMany(u => u.LiveStreamParticipations)
                            .HasForeignKey(p => p.UserId)
                            .OnDelete(DeleteBehavior.Restrict);

                     builder.HasIndex(p => new { p.LiveStreamId, p.UserId }).IsUnique(false);
              }
       }
}
