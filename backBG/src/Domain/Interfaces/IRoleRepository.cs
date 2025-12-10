using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(int id);
        Task<Role?> GetByNameAsync(string name);
        Task<List<Role>> GetAllAsync();
        Task<List<Role>> GetUserRolesAsync(int userId);
        Task AddUserRoleAsync(int userId, int roleId);
        Task RemoveUserRoleAsync(int userId, int roleId);
        Task SaveChangesAsync();
    }
}
