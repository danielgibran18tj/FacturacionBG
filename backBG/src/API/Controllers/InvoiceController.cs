using Application.Common.Interfaces;
using Application.DTOs.Invoice;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(IInvoiceService invoiceService, ILogger<InvoiceController> logger)
        {
            _invoiceService = invoiceService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInvoiceDto dto)
        {
            var result = await _invoiceService.CreateInvoiceAsync(dto);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            _logger.LogInformation("Buscando factura {InvoiceId}", id);

            var result = await _invoiceService.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _invoiceService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomer(int customerId)
        {
            return Ok(await _invoiceService.GetByCustomerAsync(customerId));
        }

        [HttpGet("seller/{sellerId}")]
        public async Task<IActionResult> GetBySeller(int sellerId)
        {
            return Ok(await _invoiceService.GetBySellerAsync(sellerId));
        }

        [HttpPost("paged")]
        public async Task<IActionResult> GetPaged([FromBody] InvoicePagedSearchDto request)
        {
            var roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            var result = await _invoiceService.GetPagedAsync(request);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> LogicalDelete(int id)
        {
            var success = await _invoiceService.LogicalDeleteAsync(id);
            if (!success)
                return NotFound(new { message = "Invoice not found" });
            return Ok(new { message = "Invoice deleted (logical delete)" });
        }


        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> GetInvoicePdf(int id)
        {
            var pdfBytes = await _invoiceService.GenerateInvoicePdfAsync(id);

            if (pdfBytes == null)
                return NotFound("Factura no encontrada");

            return File(pdfBytes, "application/pdf", $"Factura-{id}.pdf");
        }
    }
}
