using Domain.Common;

namespace Domain.Events
{
    public class ChatParticipantAddedEvent : DomainEvent
    {
        public Guid ChatId { get; }
        public Guid UserId { get; }
        public string Role { get; }

        public ChatParticipantAddedEvent(Guid chatId, Guid userId, string role)
        {
            ChatId = chatId;
            UserId = userId;
            Role = role;
        }
    }
}
