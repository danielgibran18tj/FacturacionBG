using Domain.DTOs;
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
            return await _context.Invoices
                .FirstOrDefaultAsync(i => i.Id == id);

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

        public async Task<PagedResult<Invoice>> GetPagedAsync(int page, int pageSize, string? search = null, 
            DateTime? start = null, DateTime? end = null, decimal? min = null, decimal? max = null, int? idCustomer = null)
        {
            var query = _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.Seller)
                .Include(i => i.InvoiceDetails).ThenInclude(d => d.Product)
                .Include(i => i.InvoicePaymentMethods).ThenInclude(pm => pm.PaymentMethod)
                .AsQueryable();

            // ---- FILTROS ----
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(i =>
                    i.InvoiceNumber.Contains(search) ||
                    i.Customer.FullName.Contains(search) ||
                    i.Customer.DocumentNumber.Contains(search) ||
                    i.Seller.Username.Contains(search)
                );
            }

            if (start.HasValue) query = query.Where(i => i.InvoiceDate >= start);
            if (end.HasValue) query = query.Where(i => i.InvoiceDate <= end);
            if (min.HasValue) query = query.Where(i => i.Total >= min);
            if (max.HasValue) query = query.Where(i => i.Total <= max);

            query = query.Where(i => i.CustomerId == idCustomer);

            // ---- TOTAL ----
            var totalItems = await query.CountAsync();

            // ---- PAGINACIÓN ----
            var items = await query
                .OrderByDescending(i => i.InvoiceDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Invoice>
            {
                Items = items,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                Page = page,
                PageSize = pageSize
            };
        }

    }
}
