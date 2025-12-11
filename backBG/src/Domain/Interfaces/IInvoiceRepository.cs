using Domain.DTOs;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<Invoice?> GetByIdAsync(int id);
        Task<PagedResult<Invoice>> GetPagedAsync(int page, int pageSize, string? search = null, DateTime? start = null,
            DateTime? end = null, decimal? min = null, decimal? max = null);
        Task<Invoice?> GetByIdWithDetailsAsync(int id);
        Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber);
        Task<List<Invoice>> GetAllAsync();
        Task<List<Invoice>> GetByCustomerIdAsync(int customerId);
        Task<List<Invoice>> GetBySellerIdAsync(int sellerId);
        Task<List<Invoice>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<Invoice> AddAsync(Invoice invoice);
        Task UpdateAsync(Invoice invoice);
        Task<string> GenerateInvoiceNumberAsync();
        Task SaveChangesAsync();
        Task AddDetailAsync(InvoiceDetail detail);
        Task AddPaymentAsync(InvoicePaymentMethod payment);
    }
}
