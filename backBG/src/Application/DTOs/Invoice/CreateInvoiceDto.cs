using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Invoice
{
    public class CreateInvoiceDto
    {
        public int CustomerId { get; set; }
        public int SellerId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string? Notes { get; set; }
        public List<CreateInvoiceDetailDto> Details { get; set; } = new();
        public List<CreateInvoicePaymentDto> Payments { get; set; } = new();
    }
}
