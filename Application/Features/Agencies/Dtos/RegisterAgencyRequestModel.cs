using Application.Common.Dtos;
using Application.Features.Users.Dtos;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Application.Features.Agencies.Dtos
{
    public record RegisterAgencyRequestModel(
        RegisterUserRequestModel RegisterUserRequest,
        string AgencyName, string AgencyEmail, string AgencyPhoneNumber, IFormFile? AgencyLogo,
        AddressDto? AgencyAddress);

    public record IncidentWorkTypesDto
    {
        //public IncidentType IncidentTypes { get; set; }
        //public WorkType WorkTypes { get; set; }
        public List<IncidentTypeDto> SupportedIncidents { get; set; }
        public List<WorkTypeDto> SupportedWorkTypes { get; set; }
    }

    public record RegisterAgencyFullRequestModel
    {
        public RegisterUserRequestModel RegisterUserRequest { get; set; }
        public string AgencyName { get; set; }
        public string AgencyEmail { get; set; }
        public string AgencyPhoneNumber { get; set; }
        public IFormFile? AgencyLogo { get; set; }
        public AddressDto? AgencyAddress { get; set; }

        public List<string> SupportedIncidents { get; set; }
        public List<string> SupportedWorkTypes { get; set; }

        [JsonIgnore]
         public   List<IncidentType> IncidentTypesEnums =>
        ParseIncidentTypeList(SupportedIncidents);

            [JsonIgnore]
         public   List<WorkType> WorkTypesEnums =>
            ParseWorkTypeList(SupportedWorkTypes);

        private List<IncidentType> ParseIncidentTypeList(List<string> source)
        {
            if (source == null) return new();

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

        private List<WorkType> ParseWorkTypeList(List<string> source)
        {
            if (source == null) return new();

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


    //public class RegisterAgencyRequestModel
    //{
    //    public IFormFile? AgencyLogo { get; set; }
    //    public IFormFile? UserProfilePicture { get; set; }
    //    public string PayloadJson { get; set; } = string.Empty;
    //}
}