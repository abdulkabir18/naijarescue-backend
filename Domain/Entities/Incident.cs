using Domain.Common;
using Domain.Common.Exceptions;
using Domain.Enums;
using Domain.Events;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Incident : AuditableEntity
    {
        public string ReferenceCode { get; private set; } = default!;
        public IncidentType Type { get; private set; }
        public IncidentStatus Status { get; private set; }
        public GeoLocation Location { get; private set; }
        public Address? Address { get; private set; }
        public DateTime OccurredAt { get; private set; }
        public bool IsAnonymous { get; private set; }

        public Guid? UserId { get; private set; }
        public User? User { get; private set; }

        public Guid? ChatId { get; private set; }
        public Chat? Chat { get; private set; }

        public ReporterDetails? Reporter { get; private set; }
        public VictimDetails? Victim { get; private set; }

        public ICollection<IncidentResponder> AssignedResponders { get; private set; } = [];
        public ICollection<IncidentLiveStream> LiveStreams { get; private set; } = [];
        public ICollection<IncidentMedia> IncidentMedias { get; private set; } = [];
        public ICollection<IncidentLocationUpdate> LocationUpdates { get; private set; } = [];

        private Incident() { }

        public Incident(IncidentType type, GeoLocation location, DateTime occurredAt, bool isAnonymous, ReporterDetails? reporter, Guid? userId = null, VictimDetails? victim = null)
        {
            Type = type;
            Status = IncidentStatus.Pending;
            Location = location;
            OccurredAt = occurredAt;
            IsAnonymous = isAnonymous;
            UserId = userId;
            Reporter = reporter;
            Victim = victim;
            ReferenceCode = GenerateReferenceCode();

            AddDomainEvent(new IncidentCreatedEvent(Id, UserId, type, Location, Address));
        }

        private static string GenerateReferenceCode()
        {
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            var randomPart = Guid.NewGuid().ToString("N")[..6].ToUpper();
            return $"INC-{datePart}-{randomPart}";
        }

        public void AssignResponder(Guid responderId, ResponderRole role)
        {
            if (Status is IncidentStatus.Resolved or IncidentStatus.Cancelled)
                throw new BusinessRuleException("Cannot assign responder to a resolved or cancelled incident.");

            if (AssignedResponders.Any(r => r.ResponderId == responderId && r.IsActive))
                throw new BusinessRuleException("This responder is already assigned to the incident.");

            AssignedResponders.Add(new IncidentResponder(Id, responderId, role));

            if (Status == IncidentStatus.Pending)
                Status = IncidentStatus.Reported;

            AddDomainEvent(new ResponderAssignedToIncidentEvent(Id, responderId, role));
        }

        public void UpdateAddress(Address address) => Address = address;

        public void AddLocationUpdate(GeoLocation newLocation)
        {
            if (Status is IncidentStatus.Resolved or IncidentStatus.Cancelled)
                throw new InvalidOperationException("Cannot update location for a closed incident.");

            LocationUpdates.Add(new IncidentLocationUpdate(Id, newLocation));
            Location = newLocation;

            AddDomainEvent(new IncidentLocationUpdatedEvent(Id, newLocation));
        }

        public void MarkInProgress()
        {
            if (Status != IncidentStatus.Reported)
                throw new InvalidOperationException("Incident must be reported before it can be marked in progress.");

            Status = IncidentStatus.InProgress;
            AddDomainEvent(new IncidentStatusChangedEvent(Id, Status));
        }

        public void MarkResolved()
        {
            if (Status != IncidentStatus.InProgress)
                throw new InvalidOperationException("Incident must be in progress before it can be resolved.");

            Status = IncidentStatus.Resolved;
            AddDomainEvent(new IncidentStatusChangedEvent(Id, Status));
        }

        public void Cancel()
        {
            if (Status == IncidentStatus.Resolved)
                throw new InvalidOperationException("Cannot cancel a resolved incident.");

            Status = IncidentStatus.Cancelled;
            AddDomainEvent(new IncidentStatusChangedEvent(Id, Status));
        }

        public void AddMedia(string fileUrl, MediaType mediaType)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
                throw new ArgumentException("File URL cannot be null or empty.", nameof(fileUrl));

            if (!Enum.IsDefined(typeof(MediaType), mediaType))
                throw new ArgumentException("Invalid media type.", nameof(mediaType));

            IncidentMedias.Add(new IncidentMedia(Id, fileUrl, mediaType));

            AddDomainEvent(new IncidentMediaAddedEvent(Id, fileUrl, mediaType));
        }

        public IncidentLiveStream StartLiveStream(string streamKey)
        {
            if (string.IsNullOrWhiteSpace(streamKey))
                throw new ArgumentException("Stream key cannot be empty.", nameof(streamKey));

            if (Status is IncidentStatus.Resolved or IncidentStatus.Cancelled)
                throw new InvalidOperationException("Cannot start a live stream for a resolved or cancelled incident.");

            if (LiveStreams.Any(ls => ls.EndedAt == null))
                throw new InvalidOperationException("There is already an active live stream for this incident.");

            var liveStream = new IncidentLiveStream(Id, streamKey);
            LiveStreams.Add(liveStream);

            AddDomainEvent(new IncidentLiveStreamStartedEvent(Id, liveStream.Id));

            return liveStream;
        }

        public void EndLiveStream(Guid liveStreamId)
        {
            var liveStream = LiveStreams.FirstOrDefault(ls => ls.Id == liveStreamId);
            if (liveStream == null)
                throw new InvalidOperationException("Live stream not found for this incident.");

            liveStream.EndStream();

            AddDomainEvent(new IncidentLiveStreamEndedEvent(Id, liveStreamId));
        }

        public void SetReporterDetails(string name, string phoneNumber, string email)
        {
            Reporter = new ReporterDetails(name, new PhoneNumber(phoneNumber), new Email(email));
        }

        public void SetVictimDetails(string name, string phoneNumber, string email, string description)
        {
            Victim = new VictimDetails(name, new PhoneNumber(phoneNumber), new Email(email), description);
        }

    }
}
