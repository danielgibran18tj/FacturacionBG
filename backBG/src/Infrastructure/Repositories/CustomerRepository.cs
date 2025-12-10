using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CustomerRepository> _logger;

        public CustomerRepository(ApplicationDbContext context, ILogger<CustomerRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            _logger.LogDebug("Getting customer by ID: {CustomerId}", id);
            return await _context.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Customer?> GetByDocumentNumberAsync(string documentNumber)
        {
            _logger.LogDebug("Getting customer by document: {DocumentNumber}", documentNumber);
            return await _context.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.DocumentNumber == documentNumber);
        }

        public async Task<Customer?> GetByUserIdAsync(int userId)
        {
            _logger.LogDebug("Getting customer by user ID: {UserId}", userId);
            return await _context.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<List<Customer>> GetAllAsync(bool includeInactive = false)
        {
            _logger.LogDebug("Getting all customers (includeInactive: {IncludeInactive})", includeInactive);

            var query = _context.Customers.AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(c => c.IsActive);
            }

            return await query
                .OrderBy(c => c.FullName)
                .ToListAsync();
        }

        public async Task<List<Customer>> SearchAsync(string searchTerm)
        {
            _logger.LogDebug("Searching customers with term: {SearchTerm}", searchTerm);

            return await _context.Customers
                .Where(c => c.IsActive &&
                    (c.FullName.Contains(searchTerm) ||
                     c.DocumentNumber.Contains(searchTerm) ||
                     (c.Email != null && c.Email.Contains(searchTerm)) ||
                     (c.Phone != null && c.Phone.Contains(searchTerm))))
                .OrderBy(c => c.FullName)
                .Take(50)
                .ToListAsync();
        }

        public async Task<Customer> AddAsync(Customer customer)
        {
            _logger.LogInformation("Adding customer: {DocumentNumber}", customer.DocumentNumber);
            await _context.Customers.AddAsync(customer);
            return customer;
        }

        public Task UpdateAsync(Customer customer)
        {
            _logger.LogInformation("Updating customer: {CustomerId}", customer.Id);
            customer.UpdatedAt = DateTime.UtcNow;
            _context.Customers.Update(customer);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsByDocumentNumberAsync(string documentNumber)
        {
            return await _context.Customers.AnyAsync(c => c.DocumentNumber == documentNumber);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
