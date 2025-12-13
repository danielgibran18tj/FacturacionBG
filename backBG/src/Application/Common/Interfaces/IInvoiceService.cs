using Application.DTOs;
using Application.DTOs.Invoice;
using Domain.DTOs;

namespace Application.Common.Interfaces
{
    public interface IInvoiceService
    {
        Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto dto);
        Task<PagedResult<InvoiceDto>> GetPagedAsync(InvoicePagedSearchDto request, int? idCustomer = null);
        Task<InvoiceDto?> GetByIdAsync(int id);
        Task<List<InvoiceDto>> GetAllAsync();
        Task<List<InvoiceDto>> GetByCustomerAsync(int customerId);
        Task<List<InvoiceDto>> GetBySellerAsync(int sellerId);
        Task<bool> LogicalDeleteAsync(int id);
        Task<byte[]> GenerateInvoicePdfAsync(int id);
    }
}
