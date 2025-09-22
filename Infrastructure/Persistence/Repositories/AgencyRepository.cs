using Application.Interfaces.Repositories;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class AgencyRepository : GenericRepository<Agency>, IAgencyRepository
    {
        public AgencyRepository()
        {
            
        }
        public Task<Agency?> GetAsync(Expression<Func<Agency, bool>> expression)
        {
            throw new NotImplementedException();
        }
    }
}
