using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.DTOs;
using Application.DTOs.Customer;
using AutoMapper;
using Domain.DTOs;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CustomerService> _logger;
        private readonly IMapper _mapper;

        public CustomerService(
            ICustomerRepository customerRepository,
            IMapper mapper,
            IUserRepository userRepository,
            ILogger<CustomerService> logger)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<List<CustomerDto>> GetAllAsync()
        {
            var customers = await _customerRepository.GetAllAsync();
            return _mapper.Map<List<CustomerDto>>(customers);

        }

        public async Task<CustomerDto> GetByIdAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Cliente no encontrado");

            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto> GetByDocumentNumberAsync(string documentNumber)
        {
            var customer = await _customerRepository.GetByDocumentNumberAsync(documentNumber)
                ?? throw new KeyNotFoundException("Cliente no encontrado");

            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto)
        {
            if (await _customerRepository.ExistsByDocumentNumberAsync(dto.DocumentNumber))
                throw new InvalidOperationException("El documento ya está registrado");

            var entity = dto.ToEntity();

            await _customerRepository.AddAsync(entity);
            await _customerRepository.SaveChangesAsync();

            return _mapper.Map<CustomerDto>(entity);
        }

        public async Task<CustomerDto> UpdateAsync(int id, UpdateCustomerDto dto)
        {
            var customer = await _customerRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Cliente no encontrado");

            dto.UpdateEntity(customer);

            await _customerRepository.UpdateAsync(customer);
            await _customerRepository.SaveChangesAsync();

            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task AssignUserAsync(int customerId, int userId)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId)
                ?? throw new KeyNotFoundException("Cliente no encontrado");

            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException("Usuario no encontrado");

            customer.UserId = user.Id;
            customer.UpdatedAt = DateTime.UtcNow;

            await _customerRepository.UpdateAsync(customer);
            await _customerRepository.SaveChangesAsync();
        }

        public async Task<PagedResult<CustomerDto>> GetPagedAsync(PageRequestDto request)
        {
            var pagedData = await _customerRepository.GetPagedAsync(
                request.Page,
                request.PageSize,
                request.SearchTerm
            );

            return new PagedResult<CustomerDto>
            {
                Items = _mapper.Map<List<CustomerDto>>(pagedData.Items),
                TotalItems = pagedData.TotalItems,
                TotalPages = pagedData.TotalPages,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }

    }
}
