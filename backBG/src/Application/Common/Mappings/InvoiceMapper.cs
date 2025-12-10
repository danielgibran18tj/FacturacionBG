using Application.DTOs.Invoice;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Mappings
{
    public static class InvoiceMapper
    {
        public static InvoiceDto ToDto(this Invoice invoice)
        {
            return new InvoiceDto
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                InvoiceDate = invoice.InvoiceDate,
                CustomerName = invoice.Customer.FullName,
                CustomerDocument = invoice.Customer.DocumentNumber,
                SellerName = invoice.Seller.FullName,
                Subtotal = invoice.Subtotal,
                TaxIva = invoice.TaxIva,
                Total = invoice.Total,
                Notes = invoice.Notes,
                Status = invoice.Status,
                Details = invoice.InvoiceDetails?.Select(d => d.ToDto()).ToList() ?? new(),
                Payments = invoice.InvoicePaymentMethods?.Select(p => p.ToDto()).ToList() ?? new()
            };
        }

        public static InvoiceDetailDto ToDto(this InvoiceDetail detail)
        {
            return new InvoiceDetailDto
            {
                Id = detail.Id,
                ProductId = detail.ProductId,
                ProductCode = detail.Product?.Code ?? string.Empty,
                ProductName = detail.Product?.Name ?? string.Empty,
                Quantity = detail.Quantity,
                UnitPrice = detail.UnitPrice,
                Subtotal = detail.Subtotal
            };
        }

        public static InvoicePaymentDto ToDto(this InvoicePaymentMethod payment)
        {
            return new InvoicePaymentDto
            {
                PaymentMethodId = payment.PaymentMethodId,
                PaymentMethodName = payment.PaymentMethod?.Name ?? string.Empty,
                Amount = payment.Amount
            };
        }
    }
}
