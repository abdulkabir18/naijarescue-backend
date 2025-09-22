using Domain.Enums;

namespace Domain.Entities
{
    public class ResponderSupportedIncident
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public IncidentType Type { get; private set; }
        public Guid ResponderId { get; private set; }
        public Responder Responder { get; private set; } = default!;

        private ResponderSupportedIncident() { }

        public ResponderSupportedIncident(Guid responderId, IncidentType type)
        {
            ResponderId = responderId;
            Type = type;
        }
    }
}