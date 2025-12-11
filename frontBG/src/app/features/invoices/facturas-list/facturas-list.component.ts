import { Component } from '@angular/core';
import { InvoiceService } from '../../../core/services/invoice.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Invoice } from '../../../core/models/invoice.model';
import { InvoiceDetailsModalComponent } from "../invoice-details-modal/invoice-details-modal.component";
import { DeleteConfirmModalComponent } from "../delete-confirm-modal/delete-confirm-modal.component";

@Component({
  selector: 'app-facturas-list',
  imports: [CommonModule, FormsModule, InvoiceDetailsModalComponent, DeleteConfirmModalComponent],
  templateUrl: './facturas-list.component.html',
  styleUrl: './facturas-list.component.css'
})
export class FacturasListComponent {

  invoices: Invoice[] = [];
  selectedInvoice!: Invoice | null;
  showDetails = false;
  showDelete = false;

  page = 1;
  pageSize = 10;
  totalPages = 0;
  totalItems = 0;

  searchTerm = '';

  startDate: string | null = null;
  endDate: string | null = null;

  minAmount: number | null = null;
  maxAmount: number | null = null;

  constructor(private invoiceService: InvoiceService) { }

  ngOnInit(): void {
    this.loadInvoices();
  }

  loadInvoices() {
    const body = {
      page: this.page,
      pageSize: this.pageSize,
      searchTerm: this.searchTerm,
      startDate: this.startDate,
      endDate: this.endDate,
      minAmount: this.minAmount,
      maxAmount: this.maxAmount
    };

    this.invoiceService.getPagedInvoices(body).subscribe({
      next: res => {
        this.invoices = res.items;
        this.totalItems = res.totalItems;
        this.totalPages = res.totalPages;
      },
      error: err => console.error(err)
    });
  }

  applyFilters() {
    this.page = 1;
    this.loadInvoices();
  }

  clearFilters() {
    this.startDate = null;
    this.endDate = null;
    this.minAmount = null;
    this.maxAmount = null;
    this.searchTerm = '';
    this.applyFilters();
  }

  changePage(page: number) {
    if (page < 1 || page > this.totalPages) return;
    this.page = page;
    this.loadInvoices();
  }

  downloadPdf(invoiceId: number) {
    this.invoiceService.downloadInvoicePdf(invoiceId).subscribe({
      next: (blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `Factura-${invoiceId}.pdf`;
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: err => console.error("Error al descargar PDF:", err)
    });
  }

  openDetails(invoice: Invoice) {
    this.selectedInvoice = invoice;
    this.showDetails = true;
  }

  closeDetails() {
    this.showDetails = false;
    this.selectedInvoice = null;
  }

  openDelete(invoice: Invoice) {
    this.selectedInvoice = invoice;
    this.showDelete = true;
  }

  closeDelete() {
    this.showDelete = false;
    this.selectedInvoice = null;
  }


  deleteInvoice(id: number) {
    this.invoiceService.delete(id).subscribe(() => this.loadInvoices());
  }

}