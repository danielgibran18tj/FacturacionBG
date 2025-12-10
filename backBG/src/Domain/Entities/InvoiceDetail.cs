using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class InvoiceDetail
{
    public int Id { get; set; }

    public int InvoiceId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Subtotal { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
