using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class InvoicePaymentMethod
{
    public int Id { get; set; }

    public int InvoiceId { get; set; }

    public int PaymentMethodId { get; set; }

    public decimal Amount { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;

    public virtual PaymentMethod PaymentMethod { get; set; } = null!;
}
