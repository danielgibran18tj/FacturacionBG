import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Invoice } from '../../../core/models/invoice.model';

@Component({
  selector: 'app-delete-confirm-modal',
  imports: [],
  templateUrl: './delete-confirm-modal.component.html',
  styleUrl: './delete-confirm-modal.component.css'
})
export class DeleteConfirmModalComponent {
  @Input() invoice!: Invoice | null;
  @Output() confirm = new EventEmitter<number>();
  @Output() close = new EventEmitter();

  onConfirm() {
    if (this.invoice) {
      this.confirm.emit(this.invoice.id);
    }
  }
}
