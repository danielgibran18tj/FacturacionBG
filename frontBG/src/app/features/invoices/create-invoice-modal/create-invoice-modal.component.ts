import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { InvoiceService } from '../../../core/services/invoice.service';
import { CustomerService } from '../../../core/services/customer.service';
import { PaymentMethodService } from '../../../core/services/payment-method.service';

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

  details = [
    { product: "", quantity: 1, unitPrice: 0 }
  ];

  subtotal = 0;
  iva = 0;
  total = 0;

  constructor(
    private customerService: CustomerService,
    private paymentMethodService: PaymentMethodService,
    private invoiceService: InvoiceService  
  ) { }

  ngOnInit() {
    this.loadPaymentMethods();
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
    this.details.push({ product: "", quantity: 1, unitPrice: 0 });
  }

  removeDetail(i: number) {
    this.details.splice(i, 1);
    this.recalculate();
  }

  recalculate() {
    this.subtotal = this.details.reduce((acc, d) => acc + d.quantity * d.unitPrice, 0);
    this.iva = +(this.subtotal * 0.15).toFixed(2);
    this.total = +(this.subtotal + this.iva).toFixed(2);
  }

  // ---- GUARDAR FACTURA ----
  saveInvoice() {
    if (!this.customer) return alert("Seleccione un cliente válido.");
    if (!this.paymentMethodId) return alert("Seleccione un método de pago.");

    const payload = {
      customerId: this.customer.id,
      date: this.invoiceDate,
      paymentMethodId: this.paymentMethodId,
      details: this.details.map(d => ({
        product: d.product,
        quantity: d.quantity,
        unitPrice: d.unitPrice,
        total: d.quantity * d.unitPrice
      }))
    };

    this.invoiceService.saveInvoices(payload).subscribe({
      next: () => {
        this.saved.emit();
        this.close.emit();
      },
      error: () => alert("Error guardando la factura.")
    });
  }

}
