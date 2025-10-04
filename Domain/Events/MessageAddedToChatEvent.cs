using Domain.Common;

namespace Domain.Events
{
    public class MessageAddedToChatEvent : DomainEvent
    {
        public Guid ChatId { get; }
        public Guid MessageId { get; }
        public Guid SenderId { get; }
        public string Content { get; }

        public MessageAddedToChatEvent(Guid chatId, Guid messageId, Guid senderId, string content)
        {
            ChatId = chatId;
            MessageId = messageId;
            SenderId = senderId;
            Content = content;
        }
    }
}