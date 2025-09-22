using Application.Common.Dtos;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Users.Dtos
{
    public record RegisterUserRequestModel(string FirstName, string LastName, string Email,string? UserName,Gender Gender, string PhoneNumber,string Password, string ConfirmPassword, IFormFile? ProfilePicture, AddressDto? Address);
}