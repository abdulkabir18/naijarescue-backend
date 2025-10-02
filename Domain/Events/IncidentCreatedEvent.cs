using Domain.Common;
using Domain.Enums;

namespace Domain.Events
{
    public class IncidentCreatedEvent : DomainEvent
    {
        public Guid IncidentId { get; }
        public Guid? CreatedByUserId { get; }
        public IncidentType IncidentType { get; }

        public IncidentCreatedEvent(Guid incidentId, Guid? createdByUserId, IncidentType incidentType)
        {
            IncidentId = incidentId;
            CreatedByUserId = createdByUserId;
            IncidentType = incidentType;
        }
    }
}
