using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Customer
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string DocumentNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public bool HasUserAccount { get; set; }
        public string? Username { get; set; }
        public bool IsActive { get; set; }
    }

}
