using Domain.Common;
using Domain.Common.Exceptions;
using Domain.Enums;
using Domain.Events;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Responder : AuditableEntity
    {
        public Guid UserId { get; private set; }
        public User User { get; private set; } = default!;
        public Guid AgencyId { get; private set; }
        public Agency Agency { get; private set; } = default!;
        // public string? BadgeNumber { get; private set; }
        // public string? Rank { get; private set; }
        public ResponderStatus Status { get; private set; }
        public GeoLocation? AssignedLocation { get; private set; }
        // public GeoLocation? CurrentLocation { get; private set; }
        public bool IsVerified { get; private set; }

        // public ICollection<ResponderSupportedWork> Capabilities { get; private set; } = [];
        // public ICollection<ResponderSupportedIncident> Specialties { get; private set; } = [];
        public ICollection<IncidentResponder> IncidentAssignments { get; private set; } = [];

        private Responder() { }

        public Responder(Guid userId, Guid agencyId)
        {
            if (userId == Guid.Empty) throw new ValidationException("UserId is required.");
            if (agencyId == Guid.Empty) throw new ValidationException("AgencyId is required.");

            UserId = userId;
            AgencyId = agencyId;
            Status = ResponderStatus.Unreachable;
            IsVerified = false;
        }

        public void SetBadgeNumber(string badgeNumber)
        {
            if (string.IsNullOrWhiteSpace(badgeNumber))
                throw new ValidationException("Badge number cannot be empty.");
            BadgeNumber = badgeNumber;
        }

        public void SetRank(string rank)
        {
            if (string.IsNullOrWhiteSpace(rank))
                throw new ValidationException("Rank cannot be empty.");

            Rank = rank;
        }

        public void UpdateResponderStatus(ResponderStatus status)
        {
            Status = status;

            AddDomainEvent(new ResponderStatusUpdatedEvent(Id, status));
        }

        public void AssignLocation(GeoLocation location)
        {
            AssignedLocation = location;
            // AddDomainEvent(new ResponderAssignedLocationEvent(Id, location));
        }

        public void UpdateCurrentLocation(GeoLocation location)
        {
            CurrentLocation = location;

            AddDomainEvent(new ResponderLocationUpdatedEvent(Id, location));
        }

        public void AddCapability(WorkType type, Guid responderId)
        {
            if (Capabilities.Any(c => c.Type == type && c.ResponderId == responderId))
                throw new BusinessRuleException($"Work type '{type}' is already supported by this responder.");

            // if (!User.Agency!.SupportedWorkTypes.Any(c => c.Type == type))
            //     throw new BusinessRuleException($"Work type '{type}' is not supported by the agency.");

            Capabilities.Add(new ResponderSupportedWork(responderId, type));
            // AddDomainEvent(new ResponderCapabilityAddedEvent(Id, type));
        }

        public void RemoveCapability(Guid workId)
        {
            var work = Capabilities.FirstOrDefault(c => c.Id == workId);
            if (work == null)
                throw new NotFoundException(nameof(ResponderSupportedWork), workId);

            Capabilities.Remove(work);
            // AddDomainEvent(new ResponderCapabilityRemovedEvent(Id, work.Type));
        }

        public void AddSpeciality(IncidentType type, Guid responderId)
        {
            if (Specialties.Any(s => s.Type == type && s.ResponderId == responderId))
                throw new BusinessRuleException($"Incident type '{type}' is already supported by this responder.");

            // if (!User.Agency!.SupportedIncidents.Any(w => w.Type == type))
            //     throw new BusinessRuleException($"Incident type '{type}' is not supported by the agency.");

            Specialties.Add(new ResponderSupportedIncident(responderId, type));
            // AddDomainEvent(new ResponderSpecialtyAddedEvent(Id, type));
        }

        public void RemoveSpeciality(Guid incidentId)
        {
            var incident = Specialties.FirstOrDefault(c => c.Id == incidentId);
            if (incident == null)
                throw new NotFoundException(nameof(ResponderSupportedIncident), incidentId);

            Specialties.Remove(incident);
            // AddDomainEvent(new ResponderSpecialtyRemovedEvent(Id, incident.Type));
        }

        public void Verify()
        {
            IsVerified = true;
            // AddDomainEvent(new ResponderVerifiedEvent(Id));
        }

        public void Unverify()
        {
            IsVerified = false;
            // AddDomainEvent(new ResponderUnverifiedEvent(Id));
        }
    }
}