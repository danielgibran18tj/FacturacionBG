using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class PaymentMethod
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<InvoicePaymentMethod> InvoicePaymentMethods { get; set; } = new List<InvoicePaymentMethod>();
}
