import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { InvoiceService } from '../../../core/services/invoice.service';
import { CustomerService } from '../../../core/services/customer.service';
import { PaymentMethodService } from '../../../core/services/payment-method.service';
import { AuthService } from '../../../core/services/auth.service';
import { debounceTime, Subject } from 'rxjs';
import { ProductService } from '../../../core/services/product.service';
import { SystemSettingsService } from '../../../core/services/system-settings.service';

@Component({
  selector: 'app-create-invoice-modal',
  imports: [CommonModule, FormsModule],
  templateUrl: './create-invoice-modal.component.html',
  styleUrl: './create-invoice-modal.component.css'
})
export class CreateInvoiceModalComponent {

  @Output() close = new EventEmitter<void>();
  @Output() saved = new EventEmitter<void>();

  customerIdNumber = "";
  customer: any = null;

  paymentMethods: any[] = [];
  paymentMethodId: any = "";

  invoiceDate = new Date().toISOString().split("T")[0];

  details: any[] = [
    { productId: null, productName: '', quantity: 1, unitPrice: 0 }
  ];

  productResults: any[][] = [];

  productSearch$ = new Subject<{ term: string, index: number }>();

  subtotal = 0;
  iva = 0;
  ivaGeneral = 0;
  total = 0;

  constructor(
    private customerService: CustomerService,
    private paymentMethodService: PaymentMethodService,
    private authService: AuthService,
    private invoiceService: InvoiceService,
    private systemSettingsService: SystemSettingsService,
    private productService: ProductService
  ) {
    this.productSearch$
      .pipe(debounceTime(300))
      .subscribe(({ term, index }) => {
        if (term.length < 1) {
          this.productResults[index] = [];
          return;
        }
        console.log("term", term);
        const params = {
          SearchTerm: term
        };

        this.productService.getPagedProducts({ params }).subscribe(res => {
          this.productResults[index] = res.items;
        });
      });
  }

  ngOnInit() {
    this.loadPaymentMethods();
    this.recalculate();
    this.getIva()
  }

  getIva() {
    this.systemSettingsService.getIva().subscribe({
      next: (ivaValue) => {
        this.ivaGeneral = ivaValue;
      },
      error: () => alert("Error capturando el iva.")
    });
  }

  searchProducts(term: string, index: number) {
    this.productSearch$.next({ term, index });
  }

  /** Al seleccionar un producto del autocomplete */
  selectProduct(product: any, index: number) {
    this.details[index].productId = product.id;
    this.details[index].productName = product.name;
    this.details[index].unitPrice = product.unitPrice;
    this.details[index].quantity = 1;

    // Cerrar lista
    this.productResults[index] = [];

    this.recalculate();
  }

  loadPaymentMethods() {
    this.paymentMethodService.getPaymentMethod().subscribe({
      next: res => this.paymentMethods = res,
      error: () => this.paymentMethods = []
    });
  }

  searchCustomer() {
    if (!this.customerIdNumber) return;

    this.customerService.getCustomerByDocument(this.customerIdNumber)
      .subscribe({
        next: res => {
          if (res) {
            this.customer = res
          } else {
            this.customer = null;
            alert('Cliente no encontrado.');
          }
        },
        error: () => {
          this.customer = null;
          alert('Error buscando cliente.');
        }
      });
  }

  // ---- DETALLES ----

  addDetail() {
    this.details.push({
      productId: null,
      productName: '',
      quantity: 1,
      unitPrice: 0
    });
  }

  removeDetail(i: number) {
    this.details.splice(i, 1);
    this.recalculate();
  }

  recalculate() {
    this.subtotal = this.details.reduce((acc, d) => acc + d.quantity * d.unitPrice, 0);
    this.iva = +(this.subtotal * (this.ivaGeneral / 100)).toFixed(2);
    this.total = +(this.subtotal + this.iva).toFixed(2);
  }

  // ---- GUARDAR FACTURA ----
  saveInvoice() {
    if (!this.customer) return alert("Seleccione un cliente válido.");
    if (!this.paymentMethodId) return alert("Seleccione un método de pago.");

    this.authService.getDataUserSession().subscribe({
      next: (userData: any) => {
        console.log(this.details);
        const payload = {
          customerId: this.customer.id,
          sellerId: userData.data.userId,
          invoiceDate: this.invoiceDate,
          notes: "",
          details: this.details.map(d => ({
            quantity: d.quantity,
            productId: d.productId,  // TODO: seleccionar producto correcto
          })),
          payments: [
            {
              paymentMethodId: this.paymentMethodId,
              amount: this.total
            }
          ]
        };
        var roles: string[] = userData.data.roles;
        if (roles.includes("Seller") || roles.includes("Administrator")) {
          this.invoiceService.saveInvoices(payload).subscribe({
            next: () => {
              this.saved.emit();
              this.close.emit();
            },
            error: () => alert("Error guardando la factura.")
          });
        } else {
          alert("No tiene permisos para crear facturas.");
        }
      },
      error: () => alert("Error obteniendo datos del vendedor.")
    });
  }


}

