namespace Application.Interfaces.CurrentUser
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        string Role { get; }
        string Email { get; }
    }
}
