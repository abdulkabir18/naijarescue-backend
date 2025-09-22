using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.ValueObjects;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Persistence.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly ProjectDbContext _dbContext;

        public UserRepository(ProjectDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<User?> GetAsync(Expression<Func<User, bool>> expression)
        {
            return _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(expression);
        }

        public Task<User?> GetUserByEmailAsync(string email)
        {
            return _dbContext.Users.FirstOrDefaultAsync(x => x.Email == new Email(email));
        }

        public Task<bool> IsEmailExistAsync(string email)
        {
            return _dbContext.Users.AsNoTracking().AnyAsync(x => x.Email == new Email(email));
        }

        public Task<bool> IsPhoneNumberExistAsync(string phoneNumber)
        {
            return _dbContext.Users.AsNoTracking().AnyAsync(x => x.PhoneNumber == new PhoneNumber(phoneNumber));
        }

        public Task<bool> IsUserNameExistAsync(string userName)
        {
            return _dbContext.Users.AsNoTracking().AnyAsync(x => x.UserName == userName);
        }
    }
}
