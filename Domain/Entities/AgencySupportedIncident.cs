using Domain.Enums;

namespace Domain.Entities
{
    public class AgencySupportedIncident
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public IncidentType Type { get; private set;}
        public Guid AgencyId { get; private set; }
        public Agency Agency { get; private set; } = default!;

        private AgencySupportedIncident() { }
        public AgencySupportedIncident(Guid agencyId, IncidentType type)
        {
            AgencyId = agencyId;
            Type = type;
        }
    }
}
