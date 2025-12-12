using Application.DTOs;
using Application.DTOs.Customer;
using Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface ICustomerService
    {
        Task<List<CustomerDto>> GetAllAsync();
        Task<PagedResult<CustomerDto>> GetPagedAsync(PageRequestDto request);
        Task<CustomerDto> GetByIdAsync(int id);
        Task<CustomerDto> GetByDocumentNumberAsync(string documentNumber);
        Task<CustomerDto> CreateAsync(CreateCustomerDto dto);
        Task<CustomerDto> UpdateAsync(int id, UpdateCustomerDto dto);
        Task AssignUserAsync(int customerId, int userId);
    }
}
