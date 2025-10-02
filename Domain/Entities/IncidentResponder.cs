using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class IncidentResponder : AuditableEntity
    {
        public Guid IncidentId { get; private set; }
        public Incident Incident { get; private set; } = default!;

        public Guid ResponderId { get; private set; }
        public Responder Responder { get; private set; } = default!;

        public ResponderRole Role { get; private set; }
        public bool IsActive { get; private set; }

        private IncidentResponder() { }

        public IncidentResponder(Guid incidentId, Guid responderId, ResponderRole role)
        {
            IncidentId = incidentId;
            ResponderId = responderId;
            Role = role;
            IsActive = true;
        }

        public void Deactivate() => IsActive = false;
    }
}
