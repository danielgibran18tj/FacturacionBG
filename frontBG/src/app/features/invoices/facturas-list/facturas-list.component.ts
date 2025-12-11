import { Component } from '@angular/core';
import { InvoiceService } from '../../../core/services/invoice.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-facturas-list',
  imports: [CommonModule, FormsModule],
  templateUrl: './facturas-list.component.html',
  styleUrl: './facturas-list.component.css'
})
export class FacturasListComponent {

  invoices: any[] = [];

  page = 1;
  pageSize = 10;
  totalPages = 0;
  totalItems = 0;

  searchTerm = '';

  constructor(private invoiceService: InvoiceService) { }

  ngOnInit(): void {
    this.loadInvoices();
  }

  loadInvoices() {
    const body = {
      page: this.page,
      pageSize: this.pageSize,
      searchTerm: this.searchTerm
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

  changePage(page: number) {
    if (page < 1 || page > this.totalPages) return;
    this.page = page;
    this.loadInvoices();
  }
}
