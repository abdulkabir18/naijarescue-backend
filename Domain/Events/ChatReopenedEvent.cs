using Domain.Common;

namespace Domain.Events
{
    public class ChatReopenedEvent : DomainEvent
    {
        public Guid ChatId { get; }

        public ChatReopenedEvent(Guid chatId)
        {
            ChatId = chatId;
        }
    }
}
