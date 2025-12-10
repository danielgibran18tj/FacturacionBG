using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Invoice
{
    public class CreateInvoicePaymentDto
    {
        public int PaymentMethodId { get; set; }
        public decimal Amount { get; set; }
    }
}
