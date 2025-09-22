using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using System.Linq.Expressions;

namespace Application.Interfaces.Repositories
{
    public interface IAgencyRepository : IGenericRepository<Agency>
    {
        Task<Agency?> GetAsync(Expression<Func<Agency,bool>> expression); 
    }
}
