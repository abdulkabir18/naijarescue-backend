using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;

namespace Infrastructure.Persistence.Repositories
{
    public class IncidentLocationUpdateRepository : GenericRepository<IncidentLocationUpdate>, IIncidentLocationUpdateRepository
    {
        private readonly ProjectDbContext _dbContext;
        public IncidentLocationUpdateRepository(ProjectDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }

}