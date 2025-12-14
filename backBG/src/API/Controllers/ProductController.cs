using Application.Common.Interfaces;
using Application.DTOs;
using Application.DTOs.Customer;
using Application.DTOs.Product;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _productService.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
        {
            var result = await _productService.GetAllAsync(includeInactive);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = $"{AppRoles.Administrator}")]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            var result = await _productService.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateProductDto dto)
        {
            var product = await _productService.UpdateAsync(id, dto);
            return Ok(product);
        }

        [HttpPatch("{id}/stock")]
        public async Task<IActionResult> UpdateStock(int id, [FromQuery] int quantityChange)
        {
            await _productService.UpdateStockAsync(id, quantityChange);
            return Ok(); // or NoContent
        }

        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStock()
        {
            var result = await _productService.GetLowStockAsync();
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] PageRequestDto request)
        {
            var result = await _productService.GetPagedAsync(request);
            return Ok(result);
        }

    }
}
