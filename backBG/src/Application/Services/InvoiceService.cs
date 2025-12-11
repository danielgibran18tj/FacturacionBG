using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.DTOs;
using Application.DTOs.Invoice;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IUserRepository _userRepo;
        private readonly IProductRepository _productRepo;
        private readonly IPaymentMethodRepository _paymentRepo;
        private readonly ISystemSettingService _settings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InvoiceService> _logger;

        public InvoiceService(
            IInvoiceRepository invoiceRepo,
            ICustomerRepository customerRepo,
            IUserRepository userRepo,
            IProductRepository productRepo,
            IPaymentMethodRepository paymentRepo,
            ISystemSettingService settings,
            IUnitOfWork unitOfWork,
            ILogger<InvoiceService> logger)
        {
            _invoiceRepo = invoiceRepo;
            _customerRepo = customerRepo;
            _userRepo = userRepo;
            _productRepo = productRepo;
            _paymentRepo = paymentRepo;
            _settings = settings;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // CREAR FACTURA CON TRANSACCIÓN CLEAN ARCHITECTURE
        public async Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto dto)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var customer = await _customerRepo.GetByIdAsync(dto.CustomerId)
                    ?? throw new KeyNotFoundException("Cliente no encontrado");

                var seller = await _userRepo.GetByIdAsync(dto.SellerId)
                    ?? throw new KeyNotFoundException("Vendedor no encontrado");

                var ivaPercent = await _settings.GetIvaAsync();

                var invoice = new Invoice
                {
                    InvoiceNumber = await _invoiceRepo.GenerateInvoiceNumberAsync(),
                    InvoiceDate = DateTime.UtcNow,
                    CustomerId = customer.Id,
                    SellerId = seller.Id,
                    Notes = dto.Notes,
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow
                };

                await _invoiceRepo.AddAsync(invoice);
                await _unitOfWork.SaveChangesAsync();

                decimal subtotal = 0;

                // Procesar Detalles
                foreach (var detail in dto.Details)
                {
                    var product = await _productRepo.GetByIdAsync(detail.ProductId)
                        ?? throw new KeyNotFoundException($"Producto no encontrado: {detail.ProductId}");

                    if (product.Stock < detail.Quantity)
                        throw new InvalidOperationException($"Stock insuficiente para: {product.Name}");

                    var lineSubtotal = detail.Quantity * product.UnitPrice;
                    subtotal += lineSubtotal;

                    var d = new InvoiceDetail
                    {
                        InvoiceId = invoice.Id,
                        ProductId = product.Id,
                        Quantity = detail.Quantity,
                        UnitPrice = product.UnitPrice,
                        Subtotal = lineSubtotal
                    };

                    await _invoiceRepo.AddDetailAsync(d);

                    // Restar stock
                    await _productRepo.UpdateStockAsync(product.Id, -detail.Quantity);
                }

                await _unitOfWork.SaveChangesAsync();

                invoice.Subtotal = subtotal;
                invoice.TaxIva = Math.Round(subtotal * (ivaPercent / 100), 2);
                invoice.Total = invoice.Subtotal + invoice.TaxIva;

                await _invoiceRepo.UpdateAsync(invoice);
                await _unitOfWork.SaveChangesAsync();

                // Procesar Pagos
                foreach (var p in dto.Payments)
                {
                    var method = await _paymentRepo.GetByIdAsync(p.PaymentMethodId)
                        ?? throw new KeyNotFoundException("Método de pago no encontrado");

                    var pay = new InvoicePaymentMethod
                    {
                        InvoiceId = invoice.Id,
                        PaymentMethodId = method.Id,
                        Amount = p.Amount
                    };

                    await _invoiceRepo.AddPaymentAsync(pay);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                var fullInvoice = await _invoiceRepo.GetByIdWithDetailsAsync(invoice.Id);
                return fullInvoice!.ToDto();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        // CONSULTAS
        public async Task<InvoiceDto?> GetByIdAsync(int id)
        {
            var invoice = await _invoiceRepo.GetByIdWithDetailsAsync(id);
            return invoice?.ToDto();
        }

        public async Task<List<InvoiceDto>> GetAllAsync()
        {
            var list = await _invoiceRepo.GetAllAsync();
            return list.Select(i => i.ToDto()).ToList();
        }

        public async Task<List<InvoiceDto>> GetByCustomerAsync(int customerId)
        {
            var list = await _invoiceRepo.GetByCustomerIdAsync(customerId);
            return list.Select(i => i.ToDto()).ToList();
        }

        public async Task<List<InvoiceDto>> GetBySellerAsync(int sellerId)
        {
            var list = await _invoiceRepo.GetBySellerIdAsync(sellerId);
            return list.Select(i => i.ToDto()).ToList();
        }

        public async Task<PagedResult<InvoiceDto>> GetPagedAsync(InvoicePagedSearchDto request)
        {
            var pagedData = await _invoiceRepo.GetPagedAsync(
                request.Page,
                request.PageSize,
                request.SearchTerm,
                request.StartDate,
                request.EndDate,
                request.MinAmount,
                request.MaxAmount
            );

            return new PagedResult<InvoiceDto>
            {
                Items = pagedData.Items.Select(i => i.ToDto()).ToList(),
                TotalItems = pagedData.TotalItems,
                TotalPages = pagedData.TotalPages,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
