using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;

namespace Infrastructure.Persistence.Repositories
{
    public class IncidentMediaRepository : GenericRepository<IncidentMedia>, IIncidentMediaRepository
    {
        private readonly ProjectDbContext _dbContext;
        public IncidentMediaRepository(ProjectDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
