using Application.DTOs.Customer;
using Domain.Entities;

namespace Application.Common.Mappings
{
    public static class CustomerMapper
    {
        public static CustomerDto ToDto(this Customer customer)
        {
            return new CustomerDto
            {
                Id = customer.Id,
                DocumentNumber = customer.DocumentNumber,
                FullName = customer.FullName,
                Phone = customer.Phone,
                Email = customer.Email,
                Address = customer.Address,
                HasUserAccount = customer.UserId.HasValue,
                Username = customer.User?.Username,
                IsActive = customer.IsActive
            };
        }

        public static Customer ToEntity(this CreateCustomerDto dto)
        {
            return new Customer
            {
                DocumentNumber = dto.DocumentNumber,
                FullName = dto.FullName,
                Phone = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        public static void UpdateEntity(this UpdateCustomerDto dto, Customer customer)
        {
            customer.FullName = dto.FullName;
            customer.Phone = dto.Phone;
            customer.Email = dto.Email;
            customer.Address = dto.Address;
            customer.IsActive = dto.IsActive;
            customer.UpdatedAt = DateTime.UtcNow;
        }
    }

}
