using Domain.Common;

namespace Domain.Events
{
    public class EmergencyContactAddedEvent : DomainEvent
    {
        public Guid UserId { get; }
        public string ContactName { get; }
        public string ContactEmail { get; }
        public string Relationship { get; }

        public EmergencyContactAddedEvent(Guid userId, string contactName, string contactEmail, string relationship)
        {
            UserId = userId;
            ContactName = contactName;
            ContactEmail = contactEmail;
            Relationship = relationship;
        }
    }
}
