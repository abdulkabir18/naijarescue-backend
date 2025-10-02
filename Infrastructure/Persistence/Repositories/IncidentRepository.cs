using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;

namespace Infrastructure.Persistence.Repositories
{
    public class IncidentRepository : GenericRepository<Incident>, IIncidentRepository
    {
        private readonly ProjectDbContext _dbContext;
        public IncidentRepository(ProjectDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

    }
}
