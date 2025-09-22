namespace Domain.Entities
{
    public class IncidentLiveStreamParticipant
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public Guid LiveStreamId { get; private set; }
        public IncidentLiveStream LiveStream { get; private set; } = default!;
        public Guid UserId { get; private set; }
        public User User { get; private set; } = default!;
        public DateTime JoinedAt { get; private set; }
        public DateTime? RejoinedAt { get; private set; }
        public DateTime? LeftAt { get; private set; }

        private IncidentLiveStreamParticipant() { }

        public IncidentLiveStreamParticipant(Guid liveStreamId, Guid userId)
        {
            if (userId == Guid.Empty) throw new ArgumentException("UserId cannot be empty.");
            if (liveStreamId == Guid.Empty) throw new ArgumentException("LiveStreamId cannot be empty.");

            LiveStreamId = liveStreamId;
            UserId = userId;
            JoinedAt = DateTime.UtcNow;
        }

        public void Rejoin()
        {
            RejoinedAt = DateTime.UtcNow;
            LeftAt = null; 
        }


        public void MarkLeft()
        {
            LeftAt = DateTime.UtcNow;
        }
    }
}