using Domain.Common;

namespace Domain.Events
{
    public class AgencyNameChangedEvent : DomainEvent
    {
        public Guid AgencyId { get; }
        public string NewName { get; }

        public AgencyNameChangedEvent(Guid agencyId, string newName)
        {
            AgencyId = agencyId;
            NewName = newName;
        }
    }
}
