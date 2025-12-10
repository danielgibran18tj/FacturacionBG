using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);
        Task<Product?> GetByCodeAsync(string code);
        Task<List<Product>> GetAllAsync(bool includeInactive = false);
        Task<List<Product>> GetLowStockProductsAsync();
        Task<List<Product>> SearchAsync(string searchTerm);
        Task<Product> AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task UpdateStockAsync(int productId, int quantity);
        Task<bool> ExistsByCodeAsync(string code);
        Task SaveChangesAsync();
    }
}
