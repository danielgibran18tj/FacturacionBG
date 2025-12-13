import { Component } from '@angular/core';
import { GenericTableComponent } from "../../../shared/components/generic-table/generic-table.component";
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CustomerService } from '../../../core/services/customer.service';
import { ActionDefinition, ColumnDefinition } from '../../../core/models/table-generica.model';

@Component({
  selector: 'app-customers-list',
  imports: [GenericTableComponent, FormsModule],
  templateUrl: './customers-list.component.html',
  styleUrl: './customers-list.component.css'
})
export class CustomersListComponent {

  customers: any[] = [];
  searchTerm = '';
  page = 1;
  pageSize = 5;
  totalPages = 0;

  selectedCustomer: any = null;
  showDelete = false;

  public customerColumns: ColumnDefinition[] = [];
  public customerActions: ActionDefinition[] = [];

  constructor(private customerService: CustomerService) { }

  ngOnInit() {
    this.defineTableStructure();
    this.loadCustomers();
  }

  loadCustomers() {
    this.customerService.getPagedCustomer(this.page, this.pageSize, this.searchTerm)
      .subscribe(res => {
        this.customers = res.items;
        this.totalPages = res.totalPages;
      });
  }

  handlePageChange(page: number) {
    this.page = page;
    this.loadCustomers();
  }

  handleActionClick(event: any) {
    this.selectedCustomer = event.row;

    if (event.action === 'delete') {
      this.showDelete = true;
    }

    if (event.action === 'edit') {
      // aquí puedes abrir modal de edición
      console.log('Editar cliente', this.selectedCustomer);
    }
  }

  closeDelete() {
    this.showDelete = false;
    this.selectedCustomer = null;
  }

  defineTableStructure() {

    this.customerColumns = [
      { key: 'documentNumber', label: 'Documento', textAlign: 'left' },
      { key: 'fullName', label: 'Nombre', textAlign: 'left' },
      { key: 'phone', label: 'Teléfono', textAlign: 'left' },
      { key: 'email', label: 'Email', textAlign: 'left' },
      {
        key: 'hasUserAccount',
        label: 'Usuario',
        type: 'boolean',
        textAlign: 'center'
      },
      {
        key: 'isActive',
        label: 'Estado',
        type: 'status',
        textAlign: 'center'
      }
    ];

    this.customerActions = [
      {
        label: 'Editar',
        icon: 'bi-pencil',
        class: 'btn-secondary',
        action: (customer: any) => this.openEdit(customer)
      },
      {
        label: 'Eliminar',
        icon: 'bi-trash',
        class: 'btn-danger',
        action: (customer: any) => this.openDelete(customer)
      }
    ];
  }

  openEdit(customer: any): void {
    throw new Error('Method not implemented.');
  }

  openDelete(customer: any): void {
    throw new Error('Method not implemented.');
  }


}