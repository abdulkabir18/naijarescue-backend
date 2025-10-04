using Domain.Common;

namespace Domain.Events
{
    public class ChatClosedEvent : DomainEvent
    {
        public Guid ChatId { get; }

        public ChatClosedEvent(Guid chatId)
        {
            ChatId = chatId;
        }
    }
}
