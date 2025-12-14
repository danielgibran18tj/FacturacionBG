import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Customer } from '../../../core/models/customer.model';
import { CustomerService } from '../../../core/services/customer.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-edit-customer-modal',
  imports: [CommonModule, FormsModule],
  templateUrl: './edit-customer-modal.component.html',
  styleUrl: './edit-customer-modal.component.css'
})
export class EditCustomerModalComponent {

  @Input() customer!: Customer | null;
  @Output() close = new EventEmitter();
  @Output() saved = new EventEmitter<void>();

  form!: Customer;

  constructor(private customerService: CustomerService) { }

  ngOnChanges(): void {
    if (this.customer) {
      // copia segura (no afecta el objeto padre)
      this.form = { ...this.customer };
    }
  }

  save() {
    if (!this.form) return;

    this.customerService.update(this.form.id, {
      fullName: this.form.fullName,
      phone: this.form.phone,
      email: this.form.email,
      address: this.form.address,
      isActive: this.form.isActive
    }).subscribe({
      next: () => {
        this.saved.emit();
        this.close.emit(true)
      },
      error: err => console.error('Error actualizando cliente', err)
    });
  }
}
