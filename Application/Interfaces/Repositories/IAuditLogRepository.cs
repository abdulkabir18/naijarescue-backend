using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IAuditLogRepository
    {
        Task AddAsync(AuditLog auditLog);
        Task<AuditLog?> GetByIdAsync(Guid id);
        Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<AuditLog>> GetAllAsync();
    }
}
