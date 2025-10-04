using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;

namespace Infrastructure.Persistence.Repositories
{
    public class IncidentLiveStreamRepository : GenericRepository<IncidentLiveStream>, IIncidentLiveStreamRepository
    {
        private readonly ProjectDbContext _dbContext;
        public IncidentLiveStreamRepository(ProjectDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
