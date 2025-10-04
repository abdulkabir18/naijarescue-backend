using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Events
{
    public class ResponderLocationUpdatedEvent : DomainEvent
    {
        public Guid ResponderId { get; }
        public GeoLocation NewLocation { get; }

        public ResponderLocationUpdatedEvent(Guid responderId, GeoLocation newLocation)
        {
            ResponderId = responderId;
            NewLocation = newLocation;
        }
    }
}
