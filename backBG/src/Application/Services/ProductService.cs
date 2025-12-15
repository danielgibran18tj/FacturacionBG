using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.DTOs;
using Application.DTOs.Customer;
using Application.DTOs.Product;
using AutoMapper;
using Domain.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using static iText.IO.Util.IntHashtable;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<ProductService> _logger;
        private readonly IMapper _mapper;

        public ProductService(
            IProductRepository repository,
            IMapper mapper,
            ILogger<ProductService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProductDto> GetByIdAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Producto no encontrado");

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<List<ProductDto>> GetAllAsync(bool includeInactive = false)
        {
            var products = await _repository.GetAllAsync(includeInactive);
            return products.Select(p => _mapper.Map<ProductDto>(p)).ToList();
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            if (await _repository.ExistsByCodeAsync(dto.Code))
                throw new InvalidOperationException("El código ya existe");

            var entity = _mapper.Map<Product>(dto);

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            return _mapper.Map<ProductDto>(entity);
        }

        public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto)
        {
            var product = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Producto no encontrado");

            dto.UpdateEntity(product);

            await _repository.UpdateAsync(product);
            await _repository.SaveChangesAsync();

            return _mapper.Map<ProductDto>(product);
        }

        public async Task UpdateStockAsync(int id, int quantityChange)
        {
            await _repository.UpdateStockAsync(id, quantityChange);
            await _repository.SaveChangesAsync();
        }

        public async Task<List<ProductDto>> GetLowStockAsync()
        {
            var lowStock = await _repository.GetLowStockProductsAsync();
            return lowStock.Select(p => _mapper.Map<ProductDto>(p)).ToList();
        }

        public async Task<PagedResult<ProductDto>> GetPagedAsync(PageRequestDto request)
        {
            var pagedData = await _repository.GetPagedAsync(
                           request.Page,
                           request.PageSize,
                           request.SearchTerm
                       );

            return new PagedResult<ProductDto>
            {
                Items = _mapper.Map<List<ProductDto>>(pagedData.Items),
                TotalItems = pagedData.TotalItems,
                TotalPages = pagedData.TotalPages,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
