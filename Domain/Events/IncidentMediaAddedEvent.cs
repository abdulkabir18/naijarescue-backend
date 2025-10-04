using Domain.Common;
using Domain.Enums;

namespace Domain.Events
{
    public class IncidentMediaAddedEvent : DomainEvent
    {
        public Guid IncidentId { get; }
        public string FileUrl { get; }
        public MediaType MediaType { get; }

        public IncidentMediaAddedEvent(Guid incidentId, string fileUrl, MediaType mediaType)
        {
            IncidentId = incidentId;
            FileUrl = fileUrl;
            MediaType = mediaType;
        }
    }
}
