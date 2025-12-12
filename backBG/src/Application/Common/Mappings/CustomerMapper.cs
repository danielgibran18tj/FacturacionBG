using Application.DTOs.Customer;
using Domain.Entities;

namespace Application.Common.Mappings
{
    public static class CustomerMapper
    {

        public static Customer ToEntity(this CreateCustomerDto dto)
        {
            return new Customer
            {
                DocumentNumber = dto.DocumentNumber,
                FullName = dto.FullName,
                Phone = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,
                IsActive = true
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
