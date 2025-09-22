using Domain.Enums;

namespace Domain.Entities
{
    public class ResponderSupportedWork
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public WorkType Type { get; private set; }
        public Guid ResponderId { get; private set; }
        public Responder Responder { get; private set; } = default!;

        public ResponderSupportedWork() { }

        public ResponderSupportedWork(Guid responderId, WorkType type)
        {
            ResponderId = responderId;
            Type = type;
        }
    }
}