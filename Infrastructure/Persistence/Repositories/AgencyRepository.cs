using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Persistence.Repositories
{
    public class AgencyRepository : GenericRepository<Agency>, IAgencyRepository
    {
        private readonly ProjectDbContext _dbContext;
        public AgencyRepository(ProjectDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public Task<Agency?> GetAsync(Expression<Func<Agency, bool>> expression)
        {
            return _dbContext.Agencies.AsNoTracking().FirstOrDefaultAsync(expression);
        }
    }
}
