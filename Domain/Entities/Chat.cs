using Domain.Common;
using Domain.Enums;

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
        }

        public void AddParticipant(Guid userId, string role)
        {
            if (!IsActive) throw new InvalidOperationException("Cannot add participants to a closed chat.");

            if (!Participants.Any(p => p.UserId == userId))
                Participants.Add(new ChatParticipant(userId, role));
        }

        public void AddMessage(Message message)
        {
            if (!IsActive) throw new InvalidOperationException("Cannot send messages to a closed chat.");

            Messages.Add(message);
        }

        public void CloseChat() => IsActive = false;
        public void ReopenChat() => IsActive = true;
    }
}
