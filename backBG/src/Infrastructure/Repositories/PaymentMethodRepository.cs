using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public class PaymentMethodRepository : IPaymentMethodRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PaymentMethodRepository> _logger;

        public PaymentMethodRepository(ApplicationDbContext context, ILogger<PaymentMethodRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PaymentMethod?> GetByIdAsync(int id)
        {
            return await _context.PaymentMethods.FindAsync(id);
        }

        public async Task<List<PaymentMethod>> GetAllAsync(bool includeInactive = false)
        {
            var query = _context.PaymentMethods.AsQueryable();

            if (!includeInactive)
                query = query.Where(p => p.IsActive);

            return await query.OrderBy(p => p.Name).ToListAsync();
        }

        public async Task<PaymentMethod> AddAsync(PaymentMethod paymentMethod)
        {
            await _context.PaymentMethods.AddAsync(paymentMethod);
            return paymentMethod;
        }

        public Task UpdateAsync(PaymentMethod paymentMethod)
        {
            _context.PaymentMethods.Update(paymentMethod);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
