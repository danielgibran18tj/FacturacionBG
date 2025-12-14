using Domain.DTOs;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdWithRolesAsync(int id);
        Task<bool> ExistsByUsernameAsync(string username);
        Task<bool> ExistsByEmailAsync(string email);
        Task<PagedResult<User>> GetPagedAsync(int page, int pageSize, bool IsActive, string? search = null);
        Task<User> AddAsync(User user);
        Task UpdateAsync(User user);
        Task<List<User>> GetAllAsync();
        Task<List<User>> GetSellersAsync();
        Task SaveChangesAsync();
    }
}
