namespace Application.DTOs.Customer
{
    public class CreateCustomerDto
    {
        public string DocumentNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public bool CreateUserAccount { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
