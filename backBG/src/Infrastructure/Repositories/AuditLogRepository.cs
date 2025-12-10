using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuditLogRepository> _logger;

        public AuditLogRepository(ApplicationDbContext context, ILogger<AuditLogRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<AuditLog>> GetByUserIdAsync(int userId, int limit = 100)
        {
            return await _context.AuditLogs
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Timestamp)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetByEntityAsync(string entityName, int? entityId = null, int limit = 100)
        {
            var query = _context.AuditLogs
                .Where(a => a.EntityName == entityName);

            if (entityId.HasValue)
                query = query.Where(a => a.EntityId == entityId.Value);

            return await query
                .OrderByDescending(a => a.Timestamp)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetRecentAsync(int limit = 100)
        {
            return await _context.AuditLogs
                .OrderByDescending(a => a.Timestamp)
                .Take(limit)
                .ToListAsync();
        }

        public async Task AddAsync(AuditLog auditLog)
        {
            await _context.AuditLogs.AddAsync(auditLog);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
