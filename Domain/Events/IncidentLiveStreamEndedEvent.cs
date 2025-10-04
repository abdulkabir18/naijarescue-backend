using Domain.Common;

namespace Domain.Events
{
    public class IncidentLiveStreamEndedEvent : DomainEvent
    {
        public Guid IncidentId { get; }
        public Guid LiveStreamId { get; }

        public IncidentLiveStreamEndedEvent(Guid incidentId, Guid liveStreamId)
        {
            IncidentId = incidentId;
            LiveStreamId = liveStreamId;
        }
    }
}
