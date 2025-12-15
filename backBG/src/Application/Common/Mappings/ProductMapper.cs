using Application.DTOs.Product;
using Domain.Entities;

namespace Application.Common.Mappings
{
    public static class ProductMapper
    {
        public static void UpdateEntity(this UpdateProductDto dto, Product product)
        {
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.UnitPrice = dto.UnitPrice;
            product.Stock = dto.Stock;
            product.MinStock = dto.MinStock;
            product.IsActive = dto.IsActive;
        }
    }
}
