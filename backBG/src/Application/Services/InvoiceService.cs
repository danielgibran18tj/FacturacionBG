using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.DTOs;
using Application.DTOs.Invoice;
using Domain.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly ICustomerService _customerService;
        private readonly IUserRepository _userRepo;
        private readonly IUserService _userService;
        private readonly IProductRepository _productRepo;
        private readonly IPaymentMethodRepository _paymentRepo;
        private readonly ISystemSettingService _settings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InvoiceService> _logger;
        private readonly CompanySettings _companySettings;

        public InvoiceService(
            IInvoiceRepository invoiceRepo,
            ICustomerRepository customerRepo,
            ICustomerService customerService,
            IUserRepository userRepo,
            IUserService userService,
            IProductRepository productRepo,
            IPaymentMethodRepository paymentRepo,
            ISystemSettingService settings,
            IUnitOfWork unitOfWork,
            ILogger<InvoiceService> logger,
            IOptions<CompanySettings> companySettings)

        {
            _invoiceRepo = invoiceRepo;
            _customerRepo = customerRepo;
            _customerService = customerService;
            _userRepo = userRepo;
            _userService = userService;
            _productRepo = productRepo;
            _paymentRepo = paymentRepo;
            _settings = settings;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _companySettings = companySettings.Value;
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

        public async Task<PagedResult<InvoiceDto>> GetPagedAsync(InvoicePagedSearchDto request, int? idUser_Customer = null)
        {

            int? idCustomer = null;
            if (idUser_Customer != null)
            {
                var user = await _userService.GetByIdAsync(idUser_Customer.Value);
                var customer = await _customerService.GetByUserNameAsync(user.Username);
                idCustomer = customer.Id; 
            }

            var pagedData = await _invoiceRepo.GetPagedAsync(
                request.Page,
                request.PageSize,
                request.SearchTerm,
                request.StartDate,
                request.EndDate,
                request.MinAmount,
                request.MaxAmount,
                idCustomer
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

        public async Task<bool> LogicalDeleteAsync(int id)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(id);

            if (invoice == null)
                return false;

            // Soft delete
            invoice.Status = "Inactive";

            await _invoiceRepo.UpdateAsync(invoice);
            return true;
        }

        public async Task<byte[]?> GenerateInvoicePdfAsync(int id)
        {
            var invoice = await _invoiceRepo.GetByIdWithDetailsAsync(id);

            if (invoice == null)
                return null;

            var dto = invoice.ToDto();

            return GeneratePdf(dto);
        }


        private byte[] GeneratePdf(InvoiceDto invoice)
        {
            using var ms = new MemoryStream();

            // Evita SmartMode (solución estable)
            var writer = new PdfWriter(ms, new WriterProperties());
            var pdf = new PdfDocument(writer);
            var doc = new Document(pdf);

            // Fuentes
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            PdfFont bold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

            doc.SetFont(font);

            // ---------- ENCABEZADO ----------
            var header = new Paragraph(_companySettings.CompanyName)
                .SetFont(bold)
                .SetFontSize(18)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(10);

            var subHeader = new Paragraph(
                "RUC: " + _companySettings.Ruc + 
                "\nTel: " + _companySettings.Tel +
                "\nCorreo: " + _companySettings.Correo)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(10)
                .SetMarginBottom(20);

            doc.Add(header);
            doc.Add(subHeader);

            // Línea divisoria
            doc.Add(new LineSeparator(new SolidLine()).SetMarginBottom(20));

            // ---------- INFO FACTURA ----------
            Table invoiceInfo = new Table(2).UseAllAvailableWidth();
            invoiceInfo.AddCell(CellNoBorder($"Factura Nº: {invoice.InvoiceNumber}", bold));
            invoiceInfo.AddCell(CellNoBorder($"Fecha: {invoice.InvoiceDate:dd/MM/yyyy}", bold));

            invoiceInfo.AddCell(CellNoBorder($"Cliente: {invoice.CustomerName}"));
            invoiceInfo.AddCell(CellNoBorder($"Documento: {invoice.CustomerDocument}"));

            invoiceInfo.AddCell(CellNoBorder($"Vendedor: {invoice.SellerName}"));
            invoiceInfo.AddCell(CellNoBorder(" "));

            doc.Add(invoiceInfo.SetMarginBottom(20));

            // ---------- TABLA DE DETALLES ----------
            Table table = new Table(new float[] { 3, 1, 1, 1 }).UseAllAvailableWidth();

            // Encabezados
            AddHeaderCell(table, "Producto");
            AddHeaderCell(table, "Cantidad");
            AddHeaderCell(table, "Precio");
            AddHeaderCell(table, "Subtotal");

            foreach (var d in invoice.Details)
            {
                table.AddCell(CellNormal(d.ProductName));
                table.AddCell(CellNormal(d.Quantity.ToString()));
                table.AddCell(CellNormal(d.UnitPrice.ToString("C2")));
                table.AddCell(CellNormal(d.Subtotal.ToString("C2")));
            }

            doc.Add(table.SetMarginBottom(20));

            // ---------- TOTALES ----------
            Table totals = new Table(2).UseAllAvailableWidth();

            totals.AddCell(CellRight("Subtotal:", bold));
            totals.AddCell(CellRight(invoice.Subtotal.ToString("C2")));

            totals.AddCell(CellRight("IVA 12%:", bold));
            totals.AddCell(CellRight(invoice.TaxIva.ToString("C2")));

            totals.AddCell(CellRight("TOTAL:", bold, 12));
            totals.AddCell(CellRight(invoice.Total.ToString("C2"), font: bold, fontSize: 12));

            doc.Add(totals.SetMarginBottom(20));

            // ---------- PAGOS ----------
            if (invoice.Payments != null && invoice.Payments.Any())
            {
                doc.Add(new Paragraph("Pagos Realizados")
                    .SetFont(bold)
                    .SetFontSize(12)
                    .SetMarginBottom(5));

                Table payTable = new Table(new float[] { 2, 1 }).UseAllAvailableWidth();

                AddHeaderCell(payTable, "Método");
                AddHeaderCell(payTable, "Monto");

                foreach (var p in invoice.Payments)
                {
                    payTable.AddCell(CellNormal(p.PaymentMethodName));
                    payTable.AddCell(CellNormal(p.Amount.ToString("C2")));
                }

                doc.Add(payTable.SetMarginBottom(20));
            }

            // ---------- NOTAS ----------
            if (!string.IsNullOrWhiteSpace(invoice.Notes))
            {
                doc.Add(new Paragraph("Notas:")
                    .SetFont(bold)
                    .SetMarginBottom(2));

                doc.Add(new Paragraph(invoice.Notes)
                    .SetFontSize(10)
                    .SetMarginBottom(20));
            }

            // ---------- PIE DE PÁGINA ----------
            doc.Add(new LineSeparator(new SolidLine()).SetMarginTop(10));
            doc.Add(new Paragraph("Gracias por su compra")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(9)
                .SetMarginTop(5));

            doc.Close();
            return ms.ToArray();
        }

        private Cell CellNoBorder(string text, PdfFont font = null, int fontSize = 10)
        {
            return new Cell()
                .Add(new Paragraph(text).SetFont(font ?? PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(fontSize))
                .SetBorder(Border.NO_BORDER);
        }

        private void AddHeaderCell(Table t, string text)
        {
            t.AddCell(new Cell()
                .Add(new Paragraph(text).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)))
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                .SetTextAlignment(TextAlignment.CENTER));
        }

        private Cell CellNormal(string text)
        {
            return new Cell().Add(new Paragraph(text).SetFontSize(10));
        }

        private Cell CellRight(string text, PdfFont font = null, int fontSize = 10)
        {
            return new Cell()
                .Add(new Paragraph(text)
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetFont(font ?? PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                .SetFontSize(fontSize));
        }


    }
}
