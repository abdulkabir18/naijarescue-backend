using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class IncidentLocationUpdate : AuditableEntity
    {
        public Guid IncidentId { get; private set; }
        public GeoLocation Location { get; private set; }
        public DateTime RecordedAt { get; private set; } = DateTime.UtcNow;

        private IncidentLocationUpdate() { }

        public IncidentLocationUpdate(Guid incidentId, GeoLocation location)
        {
            if (incidentId == Guid.Empty) throw new ArgumentException("IncidentId required.");
            IncidentId = incidentId;
            Location = location;
        }
    }
}