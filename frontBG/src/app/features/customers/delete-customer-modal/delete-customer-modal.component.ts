import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-delete-customer-modal',
  imports: [],
  templateUrl: './delete-customer-modal.component.html',
  styleUrl: './delete-customer-modal.component.css'
})
export class DeleteCustomerModalComponent {
  @Input() customer: {
    id: number;
    fullName: string;
    documentNumber: string;
  } | null = null;

  @Output() close = new EventEmitter<void>();
  @Output() confirm = new EventEmitter<number>();

  onConfirm() {
    if (this.customer) {
      this.confirm.emit(this.customer.id);
    }
  }

}
