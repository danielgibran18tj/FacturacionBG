import { Component } from '@angular/core';
import { InvoiceService } from '../../../core/services/invoice.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Invoice } from '../../../core/models/invoice.model';
import { InvoiceDetailsModalComponent } from "../invoice-details-modal/invoice-details-modal.component";
import { DeleteConfirmModalComponent } from "../delete-confirm-modal/delete-confirm-modal.component";
import { ActionDefinition, ColumnDefinition } from '../../../core/models/table-generica.model';
import { GenericTableComponent } from "../../../shared/components/generic-table/generic-table.component";
import { CreateInvoiceModalComponent } from "../create-invoice-modal/create-invoice-modal.component";
import { TokenService } from '../../../core/services/token.service';

@Component({
  selector: 'app-facturas-list',
  imports: [CommonModule, FormsModule, InvoiceDetailsModalComponent, DeleteConfirmModalComponent, GenericTableComponent, CreateInvoiceModalComponent],
  templateUrl: './facturas-list.component.html',
  styleUrl: './facturas-list.component.css'
})
export class FacturasListComponent {

  invoices: Invoice[] = [];
  rolesSession: string[] = [];

  // Variables para modales
  selectedInvoice!: Invoice | null;
  showDetails = false;
  showDelete = false;
  isCreateModalOpen = false;

  public totalItems: number = 0;
  public totalPages: number = 0;
  public page: number = 1;
  public pageSize: number = 10;

  searchTerm = '';
  public invoiceColumns: ColumnDefinition[] = [];
  public invoiceActions: ActionDefinition[] = [];

  startDate: string | null = null;
  endDate: string | null = null;
  minAmount: number | null = null;
  maxAmount: number | null = null;

  constructor(private invoiceService: InvoiceService, private tokenService: TokenService) { }

  ngOnInit(): void {
    this.defineTableStructure();
    this.loadInvoices();
    this.rolesSession = this.tokenService.getUserRoles()
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
        this.page = body.page; // Asegurar la página actual
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

  openCreateInvoice() {
    this.isCreateModalOpen = true;
  }

  deleteInvoice(id: number) {
    this.invoiceService.delete(id).subscribe(() => this.loadInvoices());
  }

  defineTableStructure() {
    // 1. Definición de Columnas
    this.invoiceColumns = [
      { key: 'invoiceNumber', label: '#', textAlign: 'left' },
      { key: 'invoiceDate', label: 'Fecha', type: 'date', format: 'dd/MM/yyyy', textAlign: 'left' },
      { key: 'customerName', label: 'Cliente', textAlign: 'left' },
      { key: 'sellerName', label: 'Vendedor', textAlign: 'left' },
      // Observa cómo usamos 'status' para el campo del badge
      { key: 'status', label: 'Estado', type: 'status', textAlign: 'center' },
      // Observa el uso de 'number' para formateo de moneda
      { key: 'total', label: 'Total', type: 'number', format: '1.2-2', textAlign: 'right' }
    ];

    this.invoiceActions = [
      {
        label: 'Detalles',
        icon: 'bi-pencil',
        class: 'btn-secondary',
        action: (invoice: any) => this.openDetails(invoice) // Referencia a la función local
      },
      {
        label: 'PDF',
        icon: 'bi-file-earmark-pdf',
        class: 'btn-info text-white',
        action: (invoice: any) => this.downloadPdf(invoice.id)
      },
      {
        label: 'Eliminar',
        icon: 'bi-trash',
        class: 'btn-danger',
        action: (invoice: any) => this.openDelete(invoice)
      }
    ];
  }

  // Método para manejar el cambio de página desde el componente genérico
  handlePageChange(newPage: number) {
    this.page = newPage;
    this.loadInvoices();
  }

  // Método para manejar las acciones desde el componente genérico
  handleActionClick(event: { action: (item: any) => void, item: any }) {
    // La acción es una referencia a las funciones definidas en `invoiceActions`
    event.action(event.item);
  }
}