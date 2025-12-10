using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Customer
{
    public int Id { get; set; }

    public string DocumentNumber { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public int? UserId { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual User? User { get; set; }
}
