namespace Application.Features.Users.Dtos
{
    public record VerifyUserEmailRequestModel (string Email, string Code);
}
