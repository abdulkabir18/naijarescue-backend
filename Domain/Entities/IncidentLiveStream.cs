using Domain.Common;

namespace Domain.Entities
{
    public class IncidentLiveStream : AuditableEntity
    {
        public Guid IncidentId { get; private set; }
        public Incident Incident { get; private set; } = default!;
        public string StreamKey { get; private set; } = default!; // Unique room/stream ID for WebRTC/Jitsi
        public DateTime StartedAt { get; private set; }
        public DateTime? EndedAt { get; private set; }

        public ICollection<IncidentLiveStreamParticipant> Participants { get; private set; } = [];

        private IncidentLiveStream() { }

        public IncidentLiveStream(Guid incidentId, string streamKey)
        {
            IncidentId = incidentId;
            StreamKey = streamKey;
            StartedAt = DateTime.UtcNow;
        }

        public void AddParticipant(Guid userId)
        {
            if (Participants.Any(p => p.UserId == userId && p.LeftAt == null))
                return;

            Participants.Add(new IncidentLiveStreamParticipant(Id, userId));
        }

        public void RemoveParticipant(Guid userId)
        {
            var participant = Participants.FirstOrDefault(p => p.UserId == userId && p.LeftAt == null);
            if (participant != null)
                participant.MarkLeft();
        }

        public void EndStream()
        {
            EndedAt = DateTime.UtcNow;
        }
    }
}
