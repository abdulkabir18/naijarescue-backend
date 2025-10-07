using Domain.Common;
using Domain.Common.Exceptions;
using Domain.Enums;
using Domain.Events;

namespace Domain.Entities
{
    public class Chat : AuditableEntity
    {
        public ChatType ChatType { get; private set; }
        public Guid? IncidentId { get; private set; }
        public bool IsActive { get; private set; }
        public Incident? Incident { get; private set; }

        public ICollection<ChatParticipant> Participants { get; private set; } = [];
        public ICollection<Message> Messages { get; private set; } = [];

        private Chat() { }

        public Chat(ChatType chatType, Guid? incidentId = null)
        {
            ChatType = chatType;
            IncidentId = incidentId;
            IsActive = true;

            AddDomainEvent(new ChatCreatedEvent(Id, chatType, incidentId));
        }

        public void AddParticipant(Guid userId, string role)
        {
            if (!IsActive)
                throw new BusinessRuleException("Cannot add participants to a closed chat.");

            if (Participants.Any(p => p.UserId == userId))
                throw new BusinessRuleException("User is already a participant in this chat.");

            var participant = new ChatParticipant(Id, userId, role);
            Participants.Add(participant);

            AddDomainEvent(new ChatParticipantAddedEvent(Id, userId, role));
        }

        public void AddMessage(Message message)
        {
            if (!IsActive)
                throw new BusinessRuleException("Cannot send messages to a closed chat.");

            Messages.Add(message);

            AddDomainEvent(new MessageAddedToChatEvent(Id, message.Id, message.SenderId, message.Content));
        }

        public void CloseChat()
        {
            if (!IsActive)
                throw new BusinessRuleException("Chat is already closed.");

            IsActive = false;
            AddDomainEvent(new ChatClosedEvent(Id));
        }

        public void ReopenChat()
        {
            if (IsActive)
                throw new BusinessRuleException("Chat is already active.");

            IsActive = true;
            AddDomainEvent(new ChatReopenedEvent(Id));
        }
    }
}