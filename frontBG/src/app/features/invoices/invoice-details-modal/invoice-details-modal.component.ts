import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Invoice } from '../../../core/models/invoice.model';
import { CommonModule } from '@angular/common';
import { GenericTableComponent } from "../../../shared/components/generic-table/generic-table.component";
import { ActionDefinition, ColumnDefinition } from '../../../core/models/table-generica.model';

@Component({
  selector: 'app-invoice-details-modal',
  imports: [CommonModule, GenericTableComponent],
  templateUrl: './invoice-details-modal.component.html',
  styleUrl: './invoice-details-modal.component.css'
})
export class InvoiceDetailsModalComponent {
  @Input() invoice!: Invoice | null;
  @Output() close = new EventEmitter();

  detailColumns: ColumnDefinition[] = [
    { key: 'productName', label: 'Producto', textAlign: 'left' },
    { key: 'productCode', label: 'Código', textAlign: 'left' },
    { key: 'quantity', label: 'Cantidad', textAlign: 'right' },
    { key: 'unitPrice', label: 'Precio', type: 'number', textAlign: 'right' },
    { key: 'subtotal', label: 'Subtotal', type: 'number', textAlign: 'right' }
  ];

  paymentColumns: ColumnDefinition[] = [
    { key: 'paymentMethodName', label: 'Método', textAlign: 'left' },
    { key: 'amount', label: 'Monto', type: 'number', textAlign: 'right' }
  ];
}
