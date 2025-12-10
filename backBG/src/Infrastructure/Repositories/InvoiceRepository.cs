using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InvoiceRepository> _logger;

        public InvoiceRepository(ApplicationDbContext context, ILogger<InvoiceRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Invoice?> GetByIdAsync(int id)
        {
            return await _context.Invoices.FindAsync(id);
        }

        public async Task<Invoice?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.Seller)
                .Include(i => i.InvoiceDetails).ThenInclude(d => d.Product)
                .Include(i => i.InvoicePaymentMethods).ThenInclude(pm => pm.PaymentMethod)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber)
        {
            return await _context.Invoices
                .FirstOrDefaultAsync(i => i.InvoiceNumber == invoiceNumber);
        }

        public async Task<List<Invoice>> GetAllAsync()
        {
            return await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.Seller)
                .Include(i => i.InvoiceDetails).ThenInclude(d => d.Product)
                .Include(i => i.InvoicePaymentMethods).ThenInclude(pm => pm.PaymentMethod)
                .OrderByDescending(i => i.InvoiceDate)
                .Take(200)
                .ToListAsync();
        }

        public async Task<List<Invoice>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.Invoices
                .Where(i => i.CustomerId == customerId)
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();
        }

        public async Task<List<Invoice>> GetBySellerIdAsync(int sellerId)
        {
            return await _context.Invoices
                .Where(i => i.SellerId == sellerId)
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();
        }

        public async Task<List<Invoice>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Invoices
                .Where(i =>
                    i.InvoiceDate >= startDate &&
                    i.InvoiceDate <= endDate
                )
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();
        }

        public async Task<List<Invoice>> SearchAsync(
            string searchTerm,
            DateTime? startDate = null,
            DateTime? endDate = null,
            decimal? minAmount = null,
            decimal? maxAmount = null)
        {
            var query = _context.Invoices
                .Include(i => i.Customer)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();

                query = query.Where(i =>
                    i.InvoiceNumber.ToLower().Contains(searchTerm) ||
                    (i.Customer != null && i.Customer.FullName.ToLower().Contains(searchTerm))
                );
            }

            if (startDate.HasValue)
                query = query.Where(i => i.InvoiceDate >= startDate);

            if (endDate.HasValue)
                query = query.Where(i => i.InvoiceDate <= endDate);

            if (minAmount.HasValue)
                query = query.Where(i => i.Total >= minAmount);

            if (maxAmount.HasValue)
                query = query.Where(i => i.Total <= maxAmount);

            return await query
                .OrderByDescending(i => i.InvoiceDate)
                .Take(200)
                .ToListAsync();
        }

        public async Task<Invoice> AddAsync(Invoice invoice)
        {
            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();

            return invoice;
        }

        public Task UpdateAsync(Invoice invoice)
        {
            invoice.UpdatedAt = DateTime.UtcNow;
            _context.Invoices.Update(invoice);
            return Task.CompletedTask;
        }

        public async Task<string> GenerateInvoiceNumberAsync()
        {
            var lastInvoice = await _context.Invoices
                .OrderByDescending(i => i.Id)
                .FirstOrDefaultAsync();

            int next = lastInvoice == null ? 1 : lastInvoice.Id + 1;

            return $"INV-{next:000000}";
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task AddDetailAsync(InvoiceDetail detail)
        {
            await _context.InvoiceDetails.AddAsync(detail);
        }

        public async Task AddPaymentAsync(InvoicePaymentMethod payment)
        {
            await _context.InvoicePaymentMethods.AddAsync(payment);
        }
    }
}
