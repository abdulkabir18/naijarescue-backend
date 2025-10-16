using Domain.Common;
using System.Linq.Expressions;

namespace Application.Common.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : AuditableEntity
    {
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task<T?> GetAsync(Guid id);
        Task<T?> GetForUpdateAsync(Guid id);
        void Attach(T entity);
        Task DeleteAsync(T entity);
        // Task<bool> ExistsAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> expression);
        Task<IEnumerable<T>> GetAllAsync();
        //Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    }
}
