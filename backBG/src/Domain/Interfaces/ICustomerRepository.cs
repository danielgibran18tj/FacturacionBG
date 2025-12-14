using Domain.DTOs;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(int id);
        Task<Customer?> GetByDocumentNumberAsync(string documentNumber);
        Task<Customer?> GetByUserNameAsync(string userName);
        Task<Customer?> GetByUserIdAsync(int userId);
        Task<List<Customer>> GetAllAsync(bool includeInactive = false);
        Task<PagedResult<Customer>> GetPagedAsync(int page, int pageSize, string? search = null);
        Task<Customer> AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task<bool> ExistsByDocumentNumberAsync(string documentNumber);
        Task SaveChangesAsync();
    }
}
