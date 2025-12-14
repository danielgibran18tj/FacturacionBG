import { Component } from '@angular/core';
import { GenericTableComponent } from "../../../shared/components/generic-table/generic-table.component";
import { FormsModule } from '@angular/forms';
import { CustomerService } from '../../../core/services/customer.service';
import { ActionDefinition, ColumnDefinition } from '../../../core/models/table-generica.model';
import { EditCustomerModalComponent } from "../edit-customer-modal/edit-customer-modal.component";
import { CommonModule } from '@angular/common';
import { Customer } from '../../../core/models/customer.model';
import { CreateCustomerModalComponent } from "../create-customer-modal/create-customer-modal.component";
import { DeleteCustomerModalComponent } from "../delete-customer-modal/delete-customer-modal.component";

@Component({
  selector: 'app-customers-list',
  imports: [GenericTableComponent, FormsModule, EditCustomerModalComponent, CommonModule, CreateCustomerModalComponent, DeleteCustomerModalComponent],
  templateUrl: './customers-list.component.html',
  styleUrl: './customers-list.component.css'
})
export class CustomersListComponent {

  customers: Customer[] = [];
  searchTerm = '';
  page = 1;
  pageSize = 5;
  totalPages = 0;

  selectedCustomer: any = null;
  showDelete = false;
  showEdit = false;
  isCreateModalOpen = false;

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

  openCreateCustomer() {
    this.isCreateModalOpen = true;
  }


  handlePageChange(page: number) {
    this.page = page;
    this.loadCustomers();
  }

  handleActionClick(event: any) {
    event.action(event.item);
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
        key: 'username',
        label: 'Usuario',
        type: 'boolean',
        textAlign: 'center'
      },
      {
        key: 'isActive',
        label: 'Estado',
        type: 'isActive',
        textAlign: 'center'
      }
    ];

    this.customerActions = [
      {
        label: 'Editar',
        icon: 'bi-pencil',
        class: 'btn-secondary',
        action: (customer: any) => this.openEdit(customer)
      // },
      // {
      //   label: 'Eliminar',
      //   icon: 'bi-trash',
      //   class: 'btn-danger',
      //   action: (customer: any) => this.openDelete(customer)
      }
    ];
  }

  openEdit(customer: any): void {
    this.selectedCustomer = customer;
    this.showEdit = true;
  }

  openDelete(customer: any): void {
    this.selectedCustomer = customer;
    this.showDelete = true;  }

  closeEdit() {
    this.showEdit = false;
    this.selectedCustomer = null;
  }

  deleteCustomer(customerId: number) {
    this.customerService.delete(customerId).subscribe(() => {
      this.showDelete = false;
      this.loadCustomers();
    });
  }

}