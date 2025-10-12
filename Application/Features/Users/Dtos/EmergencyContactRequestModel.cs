using Domain.Enums;

namespace Application.Features.Users.Dtos
{
    public record EmergencyContactRequestModel(string Name, string Email, string? PhoneNumber, RelationshipType Relationship, string? OtherRelationship);
}