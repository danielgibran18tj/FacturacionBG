using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IAuditLogRepository
    {
        Task<List<AuditLog>> GetByUserIdAsync(int userId, int limit = 100);
        Task<List<AuditLog>> GetByEntityAsync(string entityName, int? entityId = null, int limit = 100);
        Task<List<AuditLog>> GetRecentAsync(int limit = 100);
        Task AddAsync(AuditLog auditLog);
        Task SaveChangesAsync();
    }
}
