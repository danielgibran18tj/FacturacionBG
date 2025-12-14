using Domain.DTOs;
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

        public async Task<Customer?> GetByUserNameAsync(string userName)
        {
            _logger.LogDebug("Getting customer by userName: {userName}", userName);
            return await _context.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.FullName == userName);
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

        public async Task<PagedResult<Customer>> GetPagedAsync(int page, int pageSize, string? search = null)
        {
            var query = _context.Customers
                .Include(i => i.User)
                .AsQueryable();

            // ---- FILTROS ----
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c =>
                    c.IsActive &&
                    c.DocumentNumber.Contains(search) ||
                    c.FullName.Contains(search) ||
                    (c.Email != null && c.Email.Contains(search)) ||
                    (c.Phone != null && c.Phone.Contains(search))
                );
            }

            // ---- TOTAL ----
            var totalItems = await query.CountAsync();

            // ---- PAGINACIÓN ----
            var items = await query
                .OrderByDescending(i => i.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Customer>
            {
                Items = items,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<Customer> AddAsync(Customer customer)
        {
            _logger.LogInformation("Adding customer: {DocumentNumber}", customer.DocumentNumber);
            await _context.Customers.AddAsync(customer);
            return customer;
        }

        public async Task UpdateAsync(Customer customer)
        {
            _logger.LogInformation("Updating customer: {CustomerId}", customer.Id);
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
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
