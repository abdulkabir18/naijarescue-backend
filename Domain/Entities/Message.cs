using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class Message : AuditableEntity
    {
        public Guid ChatId { get; private set; }
        public Guid SenderId { get; private set; }
        public string Content { get; private set; } = string.Empty;
        public MessageType MessageType { get; private set; }
        public MessageStatus Status { get; private set; } = MessageStatus.Sent;
        public string? MediaUrl { get; private set; }
        public DateTime SentAt { get; private set; } = DateTime.UtcNow;

        public Chat Chat { get; private set; } = default!;
        public User Sender { get; private set; } = default!;

        private Message() { }

        public Message(Guid chatId, Guid senderId, string content, MessageType messageType, string? mediaUrl = null)
        {
            ChatId = chatId;
            SenderId = senderId;
            Content = content;
            MessageType = messageType;
            MediaUrl = mediaUrl;
        }

        public void MarkDelivered() => Status = MessageStatus.Delivered;
        public void MarkSeen() => Status = MessageStatus.Seen;
    }
}