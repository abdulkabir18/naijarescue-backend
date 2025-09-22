using Domain.Common;
using Domain.Enums;
using Domain.Events;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Incident : AuditableEntity
    {
        public IncidentType Type { get; private set; }
        public IncidentStatus Status { get; private set; }
        public GeoLocation Location { get; private set; }
        public Address? Address { get; private set; }
        public DateTime OccurredAt { get; private set; }
        public bool IsAnonymous { get; private set; }
        public Guid? UserId { get; private set; }
        public User? User { get; private set; }
        public string? ReporterName { get; private set; }
        public PhoneNumber? ReporterPhoneNumber { get; private set; }
        public Email? ReporterEmail { get; private set; }
        public Guid? AssignedResponderId { get; private set; }
        public Responder? AssignedResponder { get; private set; } 
        
        public ICollection<IncidentLiveStream> LiveStreams { get; private set; }
        public ICollection<IncidentMedia> IncidentMedias { get; private set; } = [];


        private Incident() { }

        public Incident(IncidentType type, GeoLocation location, DateTime occurredAt, bool isAnonymous, Guid? userId = null)
        {
            Type = type;
            Status = IncidentStatus.Pending;
            Location = location;
            OccurredAt = occurredAt;
            IsAnonymous = isAnonymous;
            UserId = userId;

            AddDomainEvent(new IncidentCreatedEvent(Id, UserId));
        }

        public void AssignResponder(Guid responderId)
        {
            if (Status == IncidentStatus.Resolved || Status == IncidentStatus.Cancelled)
                throw new InvalidOperationException("Cannot assign responder to a resolved or cancelled incident.");

            AssignedResponderId = responderId;
            Status = IncidentStatus.Reported;
        }

        public void UpdateAddress(Address address)
        {
            Address = address;
        }

        public void SetReporterDetails(string? reporterName, PhoneNumber? reporterPhoneNumber, Email? reporterEmail)
        {
            ReporterName = reporterName;
            ReporterPhoneNumber = reporterPhoneNumber;
            ReporterEmail = reporterEmail;
        }

    }
}
