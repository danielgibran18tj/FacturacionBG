using Domain.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(ApplicationDbContext context, ILogger<ProductRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product?> GetByCodeAsync(string code)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.Code == code);
        }

        public async Task<bool> ExistsByCodeAsync(string code)
        {
            return await _context.Products.AnyAsync(p => p.Code == code);
        }

        public async Task<List<Product>> GetAllAsync(bool includeInactive = false)
        {
            var query = _context.Products.AsQueryable();

            if (!includeInactive)
                query = query.Where(p => p.IsActive);

            return await query.OrderBy(p => p.Name).ToListAsync();
        }

        public async Task<List<Product>> GetLowStockProductsAsync()
        {
            return await _context.Products
                .Where(p => p.Stock <= 5 && p.IsActive)
                .OrderBy(p => p.Stock)
                .ToListAsync();
        }

        public async Task<List<Product>> SearchAsync(string searchTerm)
        {
            searchTerm = searchTerm.ToLower();

            return await _context.Products
                .Where(p =>
                    p.Name.ToLower().Contains(searchTerm) ||
                    p.Code.ToLower().Contains(searchTerm) ||
                    (p.Description != null && p.Description.ToLower().Contains(searchTerm))
                )
                .OrderBy(p => p.Name)
                .Take(100)
                .ToListAsync();
        }

        public async Task<Product> AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            return product;
        }

        public Task UpdateAsync(Product product)
        {
            product.UpdatedAt = DateTime.UtcNow;
            _context.Products.Update(product);
            return Task.CompletedTask;
        }

        public async Task UpdateStockAsync(int productId, int quantity)
        {
            var product = await GetByIdAsync(productId);
            if (product == null)
                throw new Exception("Product not found");

            product.Stock += quantity;
            product.UpdatedAt = DateTime.UtcNow;

            _context.Products.Update(product);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResult<Product>> GetPagedAsync(int page, int pageSize, string? search = null)
        {
            var query = _context.Products
                .AsQueryable();

            // ---- FILTROS ----
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    p.IsActive &&
                    p.Code.Contains(search) ||
                    p.Name.Contains(search) //||
                    //(p.Description != null && p.Description.Contains(search))
                );
            }

            // ---- TOTAL ----
            var totalItems = await query.CountAsync();

            // ---- PAGINACIÓN ----
            var items = await query
                .OrderByDescending(i => i.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Product>
            {
                Items = items,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                Page = page,
                PageSize = pageSize
            };
        }

    }
}
