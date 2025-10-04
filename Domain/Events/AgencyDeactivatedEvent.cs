using Domain.Common;

namespace Domain.Events
{
    public class AgencyDeactivatedEvent : DomainEvent
    {
        public Guid AgencyId { get; }

        public AgencyDeactivatedEvent(Guid agencyId)
        {
            AgencyId = agencyId;
        }
    }
}
