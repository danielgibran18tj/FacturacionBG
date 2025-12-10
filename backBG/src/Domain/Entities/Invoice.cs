using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Invoice
{
    public int Id { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty;

    public DateTime InvoiceDate { get; set; }

    public int CustomerId { get; set; }

    public int SellerId { get; set; }

    public decimal Subtotal { get; set; }

    public decimal TaxIva { get; set; }

    public decimal Total { get; set; }

    public string? Notes { get; set; }

    public string Status { get; set; } = "Active";

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();

    public virtual ICollection<InvoicePaymentMethod> InvoicePaymentMethods { get; set; } = new List<InvoicePaymentMethod>();

    public virtual User Seller { get; set; } = null!;
}
