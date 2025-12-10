using Application.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IProductService
    {
        Task<ProductDto> GetByIdAsync(int id);
        Task<List<ProductDto>> GetAllAsync(bool includeInactive = false);
        Task<ProductDto> CreateAsync(CreateProductDto dto);
        Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto);
        Task UpdateStockAsync(int id, int quantityChange);
        Task<List<ProductDto>> GetLowStockAsync();
    }
}
