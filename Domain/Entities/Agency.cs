using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public  class Agency : AuditableEntity
    {
        public string Name { get; private set; }

        public Email Email { get; private set; }
        public PhoneNumber PhoneNumber { get; private set; }

        public string? LogoUrl { get; private set; }
        public Address? Address { get; private set; }

        public bool IsActive { get; private set; }

        public ICollection<AgencySupportedIncident> SupportedIncidents { get; private set; } = [];
        public ICollection<AgencySupportedWork> SupportedWorkTypes { get; private set; } = [];

        public ICollection<User> Users { get; private set; } = [];
        //public ICollection<EmergencyReport> RespondedIncidents { get; private set; } = new List<EmergencyReport>();

        private Agency() { }

        public Agency(string name, Email email, PhoneNumber phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Agency name is required.");
            Name = name;
            Email = email ?? throw new ArgumentNullException(nameof(email));
            PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
            IsActive = true;
        }

        public void ChangeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Agency name is required.");
            Name = name;
        }

        public void ChangeContactInfo(Email email, PhoneNumber phoneNumber)
        {
            Email = email ?? throw new ArgumentNullException(nameof(email));
            PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        }
        public void SetLogo(string logoUrl) => LogoUrl = logoUrl;
        public void SetAddress(Address address) => Address = address;

        public void AddSupportedIncident(IncidentType type, Guid agencyId)
        {
            if (SupportedIncidents.Any(si => si.Type == type && si.AgencyId == agencyId))
                throw new InvalidOperationException($"Incident type '{type}' is already supported.");

            SupportedIncidents.Add(new AgencySupportedIncident(agencyId, type));
        }

        public void AddSupportedWorkType(WorkType type, Guid agencyId)
        {
            if (SupportedWorkTypes.Any(sw => sw.Type == type && sw.AgencyId == agencyId))
                throw new InvalidOperationException($"Work type '{type}' is already supported.");

            SupportedWorkTypes.Add(new AgencySupportedWork(agencyId, type));
        }

        public void RemoveSupportedIncident(Guid supportedIncidentId)
        {
            var incident = SupportedIncidents.FirstOrDefault(si => si.Id == supportedIncidentId);
            if (incident != null)
                SupportedIncidents.Remove(incident);
        }

        public void RemoveSupportedWorkType(Guid supportedWorkId)
        {
            var work = SupportedWorkTypes.FirstOrDefault(sw => sw.Id == supportedWorkId);
            if (work != null)
                SupportedWorkTypes.Remove(work);
        }

        public void Deactivate() => IsActive = false;
        public void Reactivate() => IsActive = true;
    }
}
