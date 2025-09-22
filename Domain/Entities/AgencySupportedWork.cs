using Domain.Enums;

namespace Domain.Entities
{
    public class AgencySupportedWork
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public WorkType Type { get; private set; }
        public Guid AgencyId { get; private set; }
        public Agency Agency { get; private set; } = default!;

        private AgencySupportedWork() { }
        public AgencySupportedWork(Guid agencyId, WorkType type)
        {
            AgencyId = agencyId;
            Type = type;
        }
    }
}
