using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class IncidentMedia : AuditableEntity
    {
        public string FileUrl { get; private set; }
        public MediaType MediaType { get; private set; }
        public Guid IncidentId { get; private set; }
        public Incident Incident { get; private set; } = default!;

        private IncidentMedia() { }

        public IncidentMedia(Guid incidentId, string fileUrl, MediaType mediaType)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
                throw new ArgumentException("File URL cannot be empty.", nameof(fileUrl));

            IncidentId = incidentId;
            FileUrl = fileUrl;
            MediaType = mediaType;
        }
    }
}
