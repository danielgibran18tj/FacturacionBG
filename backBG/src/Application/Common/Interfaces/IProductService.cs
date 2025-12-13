using Application.DTOs;
using Application.DTOs.Product;
using Domain.DTOs;

namespace Application.Common.Interfaces
{
    public interface IProductService
    {
        Task<ProductDto> GetByIdAsync(int id);
        Task<PagedResult<ProductDto>> GetPagedAsync(PageRequestDto request);
        Task<List<ProductDto>> GetAllAsync(bool includeInactive = false);
        Task<ProductDto> CreateAsync(CreateProductDto dto);
        Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto);
        Task UpdateStockAsync(int id, int quantityChange);
        Task<List<ProductDto>> GetLowStockAsync();
    }
}
