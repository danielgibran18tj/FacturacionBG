import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CreateCustomerRequest } from '../../../core/models/customer.model';
import { CustomerService } from '../../../core/services/customer.service';

@Component({
  selector: 'app-create-customer-modal',
  imports: [FormsModule, CommonModule],
  templateUrl: './create-customer-modal.component.html',
  styleUrl: './create-customer-modal.component.css'
})
export class CreateCustomerModalComponent {
  @Output() close = new EventEmitter<void>();
  @Output() created = new EventEmitter<void>();

  model: CreateCustomerRequest = {
    documentNumber: '',
    fullName: '',
    phone: '',
    email: '',
    address: ''
  };

  linkUser: boolean = false;
  isSaving = false;

  constructor(private customerService: CustomerService) { }

  save() {
    this.isSaving = true;

    const body: CreateCustomerRequest = {
      documentNumber: this.model.documentNumber,
      fullName: this.model.fullName,
      phone: this.model.phone,
      email: this.model.email,
      address: this.model.address
    };

    if (this.linkUser) {
      body.hasUserAccount = true;
      body.username = this.model.username;
    }

    console.log(body);

    this.customerService.create(body).subscribe({
      next: () => {
        this.isSaving = false;
        this.created.emit();
        this.close.emit();
      },
      error: () => this.isSaving = false
    });
    
  }

}
