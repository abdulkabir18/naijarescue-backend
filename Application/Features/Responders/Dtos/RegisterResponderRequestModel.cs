using Application.Common.Dtos;
using Application.Features.Users.Dtos;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace Application.Features.Responders.Dtos
{
    public record RegisterResponderRequestModel(
        RegisterUserRequestModel RegisterUserRequest,
        Guid AgencyId, string? BadgeNumber, string? Rank,
        GeoLocationDto? AssignedLocation);

    //public record WorkTypesDto(ICollection<WorkTypeDto> Types);
    //public record IncidentTypesDto(ICollection<IncidentTypeDto> Types);

    public record RegisterResponderFullRequestModel
    {
        public RegisterUserRequestModel RegisterUserRequest { get; set; }
        public Guid AgencyId { get; set; }
        public string? BadgeNumber { get; set; }
        public string? Rank { get; set; }
        public GeoLocationDto? AssignedLocation { get; set; }

        public List<string> Specialties { get; set; }
        public List<string> Capabilities { get; set; }

        [JsonIgnore]
        public List<IncidentType> SpecialtiesEnums =>
        ParseIncidentTypeList(Specialties);

        [JsonIgnore]
        public List<WorkType> CapabilitiesEnums =>
        ParseWorkTypeList(Capabilities);

        private static List<IncidentType> ParseIncidentTypeList(List<string> source)
        {
            if (source == null) return [];

            var results = new List<IncidentType>();
            foreach (var s in source)
            {
                if (Enum.TryParse<IncidentType>(s, true, out var status))
                {
                    results.Add(status);
                }
                else
                {
                    throw new ArgumentException($"Invalid status: '{s}'");
                }
            }

            return results;
        }

        private static List<WorkType> ParseWorkTypeList(List<string> source)
        {
            if (source == null) return [];

            var results = new List<WorkType>();
            foreach (var s in source)
            {
                if (Enum.TryParse<WorkType>(s, true, out var status))
                {
                    results.Add(status);
                }
                else
                {
                    throw new ArgumentException($"Invalid status: '{s}'");
                }
            }

            return results;
        }

    }
}