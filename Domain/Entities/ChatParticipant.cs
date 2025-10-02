using Domain.Common;

namespace Domain.Entities
{
    public class ChatParticipant : AuditableEntity
    {
        public Guid ChatId { get; private set; }
        public Guid UserId { get; private set; }
        public string Role { get; private set; }

        public Chat Chat { get; private set; } = default!;
        public User User { get; private set; } = default!;

        private ChatParticipant() { }

        public ChatParticipant(Guid userId, string role)
        {
            UserId = userId;
            Role = role;
        }
    }
}