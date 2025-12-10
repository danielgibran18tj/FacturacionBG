using Application.DTOs.Product;
using Domain.Entities;

namespace Application.Common.Mappings
{
    public static class ProductMapper
    {
        public static ProductDto ToDto(this Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Code = product.Code,
                Name = product.Name,
                Description = product.Description,
                UnitPrice = product.UnitPrice,
                Stock = product.Stock,
                MinStock = product.MinStock,
                IsActive = product.IsActive,
                IsLowStock = product.MinStock.HasValue && product.Stock <= product.MinStock.Value
            };
        }

        public static Product ToEntity(this CreateProductDto dto)
        {
            return new Product
            {
                Code = dto.Code,
                Name = dto.Name,
                Description = dto.Description,
                UnitPrice = dto.UnitPrice,
                Stock = dto.Stock,
                MinStock = dto.MinStock,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        public static void UpdateEntity(this UpdateProductDto dto, Product product)
        {
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.UnitPrice = dto.UnitPrice;
            product.Stock = dto.Stock;
            product.MinStock = dto.MinStock;
            product.IsActive = dto.IsActive;
            product.UpdatedAt = DateTime.UtcNow;
        }
    }
}
