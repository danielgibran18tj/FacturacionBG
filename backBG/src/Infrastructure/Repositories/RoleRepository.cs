using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RoleRepository> _logger;

        public RoleRepository(ApplicationDbContext context, ILogger<RoleRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Role?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Roles.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting role by ID: {RoleId}", id);
                throw;
            }
        }

        public async Task<Role?> GetByNameAsync(string name)
        {
            try
            {
                return await _context.Roles
                    .FirstOrDefaultAsync(r => r.Name == name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting role by name: {RoleName}", name);
                throw;
            }
        }

        public async Task<List<Role>> GetAllAsync()
        {
            try
            {
                return await _context.Roles
                    .OrderBy(r => r.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all roles");
                throw;
            }
        }

        public async Task<List<Role>> GetUserRolesAsync(int userId)
        {
            try
            {
                return await _context.UserRoles
                    .Where(ur => ur.UserId == userId)
                    .Select(ur => ur.Role)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting roles for user: {UserId}", userId);
                throw;
            }
        }

        public async Task AddUserRoleAsync(int userId, int roleId)
        {
            try
            {
                var userRole = new UserRole
                {
                    UserId = userId,
                    RoleId = roleId,
                    AssignedAt = DateTime.UtcNow
                };

                await _context.UserRoles.AddAsync(userRole);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding role {RoleId} to user {UserId}", roleId, userId);
                throw;
            }
        }

        public async Task RemoveUserRoleAsync(int userId, int roleId)
        {
            try
            {
                var userRole = await _context.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

                if (userRole != null)
                {
                    _context.UserRoles.Remove(userRole);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing role {RoleId} from user {UserId}", roleId, userId);
                throw;
            }
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving role changes");
                throw;
            }
        }
    }
}
