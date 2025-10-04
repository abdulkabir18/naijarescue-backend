using Domain.Common;
using Domain.Enums;

namespace Domain.Events
{
    public class ChatCreatedEvent : DomainEvent
    {
        public Guid ChatId { get; }
        public ChatType ChatType { get; }
        public Guid? IncidentId { get; }

        public ChatCreatedEvent(Guid chatId, ChatType chatType, Guid? incidentId)
        {
            ChatId = chatId;
            ChatType = chatType;
            IncidentId = incidentId;
        }
    }
}
