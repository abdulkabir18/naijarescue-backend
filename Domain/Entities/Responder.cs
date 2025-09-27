using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Responder : AuditableEntity
    {
        public Guid UserId { get; private set; }
        public User User { get; private set; } = default!;
        public Guid AgencyId { get; private set; }
        public Agency Agency { get; private set; } = default!;
        public string? BadgeNumber { get; private set; }
        public string? Rank { get; private set; }
        public ResponderStatus Status { get; private set; }
        public GeoLocation? AssignedLocation { get; private set; }
        public GeoLocation? CurrentLocation { get; private set; }
        public bool IsVerified { get; private set; }

        public ICollection<ResponderSupportedWork> Capabilities { get; private set; } = [];
        public ICollection<ResponderSupportedIncident> Specialties { get; private set; } = [];
        public ICollection<Incident> AssignedIncidents { get; private set; } = [];

        private Responder() { }

        public Responder(Guid userId, Guid agencyId)
        {
            UserId = userId;
            AgencyId = agencyId;
            Status = ResponderStatus.Unreachable;
            IsVerified = false;
        }

        public void SetBadgeNumber(string badgeNumber) => BadgeNumber = badgeNumber;
        public void SetRank(string rank) => Rank = rank;
        public void UpdateResponderStatus(ResponderStatus status) => Status = status;

        public void AssignLocation(GeoLocation location) => AssignedLocation = location;

        public void UpdateCurrentLocation(GeoLocation location)
        {
            CurrentLocation = location;
            // AddDomainEvent(new ResponderLocationUpdatedEvent(Id, location));
        }

        public void AddCapability(WorkType type, Guid responderId)
        {
            if (Capabilities.Any(c => c.Type == type && c.ResponderId == responderId))
                throw new InvalidOperationException($"Work type '{type}' is already supported.");

            if (!User.Agency!.SupportedWorkTypes.Any(c => c.Type == type))
                throw new InvalidOperationException($"Work type '{type}' is not supported by your agency");

            Capabilities.Add(new ResponderSupportedWork(responderId, type));
        }

        public void RemoveCapability(Guid workId)
        {
            var work = Capabilities.FirstOrDefault(c => c.Id == workId);
            if (work != null)
                Capabilities.Remove(work);
        }

        public void AddSpecialty(IncidentType type, Guid responderId)
        {
            if (Specialties.Any(s => s.Type == type && s.ResponderId == responderId))
                throw new InvalidOperationException($"Incident type '{type}' is already supported.");

            if (!User.Agency!.SupportedIncidents.Any(w => w.Type == type))
                throw new InvalidOperationException($"Incident type '{type}' is not supported by your agency");

            Specialties.Add(new ResponderSupportedIncident(responderId, type));
        }

        public void RemoveSpecialty(Guid incidentId)
        {
            var incident = Specialties.FirstOrDefault(c => c.Id == incidentId);
            if (incident != null)
                Specialties.Remove(incident);
        }
        public void Verify() => IsVerified = true;
        public void Unverify() => IsVerified = false;
    }
}
