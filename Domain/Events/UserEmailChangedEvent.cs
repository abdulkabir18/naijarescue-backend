using Domain.Common;

namespace Domain.Events
{
    public class UserEmailChangedEvent : DomainEvent
    {
        public Guid UserId { get; }
        public string NewEmail { get; }

        public UserEmailChangedEvent(Guid userId, string newEmail)
        {
            UserId = userId;
            NewEmail = newEmail;
        }
    }
}
