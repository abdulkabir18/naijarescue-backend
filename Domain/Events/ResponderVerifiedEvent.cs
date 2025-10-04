using Domain.Common;

namespace Domain.Events
{
    public class ResponderVerifiedEvent : DomainEvent
    {
        public Guid ResponderId { get; }

        public ResponderVerifiedEvent(Guid responderId) => ResponderId = responderId;
    }
}
