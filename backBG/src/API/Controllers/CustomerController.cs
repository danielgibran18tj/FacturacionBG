using Application.Common.Interfaces;
using Application.DTOs;
using Application.DTOs.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = $"{AppRoles.Administrator},{AppRoles.Seller}")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _customerService.GetAllAsync();
            return Ok(customers);
        }

        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        [HttpGet("documentNumber/{documentNumber}")]
        public async Task<IActionResult> GetByDocumentNumber(string documentNumber)
        {
            var customer = await _customerService.GetByDocumentNumberAsync(documentNumber);
            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCustomerDto dto)
        {
            var customer = await _customerService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateCustomerDto dto)
        {
            var customer = await _customerService.UpdateAsync(id, dto);
            return Ok(customer);
        }

        [HttpPost("{customerId}/assign-user/{userId}")]
        public async Task<IActionResult> AssignUser(int customerId, int userId)
        {
            await _customerService.AssignUserAsync(customerId, userId);
            return NoContent();
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] PageRequestDto request)
        {
            var result = await _customerService.GetPagedAsync(request);
            return Ok(result);
        }

    }
}
