using Domain.Common;

namespace Domain.Events
{
    public class IncidentCreatedEvent : DomainEvent
    {
        public Guid IncidentId { get; }
        public Guid? CreatedByUserId { get; }

        public IncidentCreatedEvent(Guid incidentId, Guid? createdByUserId)
        {
            IncidentId = incidentId;
            CreatedByUserId = createdByUserId;
        }
    }
}
