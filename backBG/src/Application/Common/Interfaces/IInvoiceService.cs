using Application.DTOs.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IInvoiceService
    {
        Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto dto);
        Task<InvoiceDto?> GetByIdAsync(int id);
        Task<List<InvoiceDto>> GetAllAsync();
        Task<List<InvoiceDto>> GetByCustomerAsync(int customerId);
        Task<List<InvoiceDto>> GetBySellerAsync(int sellerId);
        Task<List<InvoiceDto>> SearchAsync(InvoiceSearchDto dto);
    }
}
