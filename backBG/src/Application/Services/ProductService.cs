using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.DTOs.Product;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            IProductRepository repository,
            ILogger<ProductService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ProductDto> GetByIdAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Producto no encontrado");

            return product.ToDto();
        }

        public async Task<List<ProductDto>> GetAllAsync(bool includeInactive = false)
        {
            var products = await _repository.GetAllAsync(includeInactive);
            return products.Select(p => p.ToDto()).ToList();
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            if (await _repository.ExistsByCodeAsync(dto.Code))
                throw new InvalidOperationException("El código ya existe");

            var entity = dto.ToEntity();

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            return entity.ToDto();
        }

        public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto)
        {
            var product = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Producto no encontrado");

            dto.UpdateEntity(product);

            await _repository.UpdateAsync(product);
            await _repository.SaveChangesAsync();

            return product.ToDto();
        }

        public async Task UpdateStockAsync(int id, int quantityChange)
        {
            await _repository.UpdateStockAsync(id, quantityChange);
            await _repository.SaveChangesAsync();
        }

        public async Task<List<ProductDto>> GetLowStockAsync()
        {
            var lowStock = await _repository.GetLowStockProductsAsync();
            return lowStock.Select(p => p.ToDto()).ToList();
        }
    }
}
