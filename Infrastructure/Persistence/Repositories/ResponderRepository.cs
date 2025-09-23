using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;

namespace Infrastructure.Persistence.Repositories
{
    public class ResponderRepository : GenericRepository<Responder>, IResponderRepository
    {
        private readonly ProjectDbContext _dbContext;
        public ResponderRepository(ProjectDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
