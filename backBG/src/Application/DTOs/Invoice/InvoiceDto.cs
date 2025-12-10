using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Invoice
{
    public class InvoiceDto
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerDocument { get; set; } = string.Empty;
        public string SellerName { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal TaxIva { get; set; }
        public decimal Total { get; set; }
        public string? Notes { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<InvoiceDetailDto> Details { get; set; } = new();
        public List<InvoicePaymentDto> Payments { get; set; } = new();
    }
}
