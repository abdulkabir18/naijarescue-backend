using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;

namespace Infrastructure.Persistence.Repositories
{
    public class IncidentResponderRepository : GenericRepository<IncidentResponder>, IIncidentResponderRepository
    {
        private readonly ProjectDbContext _dbContext;
        public IncidentResponderRepository(ProjectDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
