using Domain.Common;

namespace Domain.Events
{
    public class IncidentLiveStreamStartedEvent : DomainEvent
    {
        public Guid IncidentId { get; }
        public Guid LiveStreamId { get; }

        public IncidentLiveStreamStartedEvent(Guid incidentId, Guid liveStreamId)
        {
            IncidentId = incidentId;
            LiveStreamId = liveStreamId;
        }
    }
}
