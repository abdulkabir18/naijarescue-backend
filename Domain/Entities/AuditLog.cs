using Domain.Enums;

namespace Domain.Entities
{
    public class AuditLog
    {
        public Guid Id { get; private init; } = Guid.NewGuid();

        public Guid? UserId { get; private init; }
        public User? User { get; private init; }

        public DateTime Timestamp { get; private init; } = DateTime.UtcNow;
        public AuditActionType Action { get; private init; }

        public string? EntityName { get; private init; }
        public Guid? EntityId { get; private init; }

        public string? Description { get; private init; }
        public string? IpAddress { get; private init; }
        public string? UserAgent { get; private init; }

        private AuditLog() { }

        public AuditLog(AuditActionType action, string? entityName, Guid? entityId, string? description, Guid? userId = null, string? ipAddress = null, string? userAgent = null)
        {
            Action = action;
            EntityName = entityName;
            EntityId = entityId;
            Description = description;
            UserId = userId;
            IpAddress = ipAddress;
            UserAgent = userAgent;
        }
    }
}
