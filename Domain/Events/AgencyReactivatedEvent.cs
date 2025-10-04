using Domain.Common;

namespace Domain.Events
{
    public class AgencyReactivatedEvent : DomainEvent
    {
        public Guid AgencyId { get; }

        public AgencyReactivatedEvent(Guid agencyId)
        {
            AgencyId = agencyId;
        }
    }
}
