using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Events
{
    public class IncidentLocationUpdatedEvent : DomainEvent
    {
        public Guid IncidentId { get; }
        public GeoLocation NewLocation { get; }

        public IncidentLocationUpdatedEvent(Guid incidentId, GeoLocation newLocation)
        {
            IncidentId = incidentId;
            NewLocation = newLocation;
        }
    }
}
