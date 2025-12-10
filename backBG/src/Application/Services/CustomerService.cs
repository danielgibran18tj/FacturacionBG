using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.DTOs.Customer;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(
            ICustomerRepository repository,
            IUserRepository userRepository,
            ILogger<CustomerService> logger)
        {
            _repository = repository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<List<CustomerDto>> GetAllAsync()
        {
            var customers = await _repository.GetAllAsync();
            return customers.Select(c => c.ToDto()).ToList();
        }

        public async Task<CustomerDto> GetByIdAsync(int id)
        {
            var customer = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Cliente no encontrado");

            return customer.ToDto();
        }

        public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto)
        {
            if (await _repository.ExistsByDocumentNumberAsync(dto.DocumentNumber))
                throw new InvalidOperationException("El documento ya está registrado");

            var entity = dto.ToEntity();

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            return entity.ToDto();
        }

        public async Task<CustomerDto> UpdateAsync(int id, UpdateCustomerDto dto)
        {
            var customer = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Cliente no encontrado");

            dto.UpdateEntity(customer);

            await _repository.UpdateAsync(customer);
            await _repository.SaveChangesAsync();

            return customer.ToDto();
        }

        public async Task AssignUserAsync(int customerId, int userId)
        {
            var customer = await _repository.GetByIdAsync(customerId)
                ?? throw new KeyNotFoundException("Cliente no encontrado");

            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException("Usuario no encontrado");

            customer.UserId = user.Id;
            customer.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(customer);
            await _repository.SaveChangesAsync();
        }
    }
}
