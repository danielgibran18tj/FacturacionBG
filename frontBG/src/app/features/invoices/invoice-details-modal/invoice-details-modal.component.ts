import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Invoice } from '../../../core/models/invoice.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-invoice-details-modal',
  imports: [CommonModule],
  templateUrl: './invoice-details-modal.component.html',
  styleUrl: './invoice-details-modal.component.css'
})
export class InvoiceDetailsModalComponent {
  @Input() invoice!: Invoice | null;
  @Output() close = new EventEmitter();


}
