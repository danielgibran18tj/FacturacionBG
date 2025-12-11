using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CompanySettings
    {
        public string CompanyName { get; set; } = string.Empty;
        public string Ruc { get; set; } = string.Empty;
        public string Tel { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
    }
}
