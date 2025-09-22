using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using System.Linq.Expressions;

namespace Application.Interfaces.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetAsync(Expression<Func<User,bool>> expression);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> IsEmailExistAsync(string email);
        Task<bool> IsPhoneNumberExistAsync(string phoneNumber);
        Task<bool> IsUserNameExistAsync(string userName);
    }
}
