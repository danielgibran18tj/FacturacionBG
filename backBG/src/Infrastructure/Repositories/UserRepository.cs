using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            _logger.LogDebug("Getting user by ID: {UserId}", id);
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            _logger.LogDebug("Getting user by username: {Username}", username);
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            _logger.LogDebug("Getting user by email: {Email}", email);
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByIdWithRolesAsync(int id)
        {
            _logger.LogDebug("Getting user with roles by ID: {UserId}", id);
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User> AddAsync(User user)
        {
            _logger.LogInformation("Adding user: {Username}", user.Username);
            await _context.Users.AddAsync(user);
            return user;
        }

        public Task UpdateAsync(User user)
        {
            _logger.LogInformation("Updating user: {UserId}", user.Id);
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            return Task.CompletedTask;
        }

        public async Task<List<User>> GetAllAsync()
        {
            _logger.LogDebug("Getting all active users");
            return await _context.Users
                .Where(u => u.IsActive)
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        public async Task<List<User>> GetSellersAsync()
        {
            _logger.LogDebug("Getting all sellers");
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Where(u => u.IsActive && u.UserRoles.Any(ur => ur.Role.Name == "Seller"))
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
