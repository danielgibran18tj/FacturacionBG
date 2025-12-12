import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ActionDefinition, ColumnDefinition } from '../../../core/models/table-generica.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-generic-table',
  imports: [CommonModule],
  templateUrl: './generic-table.component.html',
  styleUrl: './generic-table.component.css'
})
export class GenericTableComponent {
  @Input() data: any[] = [];
  @Input() columns: ColumnDefinition[] = [];
  @Input() actions: ActionDefinition[] = [];
  @Input() totalPages: number = 1;
  @Input() currentPage: number = 1;

  @Output() pageChange = new EventEmitter<number>();
  @Output() actionClick = new EventEmitter<{ action: (item: any) => void, item: any }>();

  // Helper para obtener el valor del objeto usando la key de la columna
  getCellValue(item: any, column: ColumnDefinition): any {
    return item[column.key];
  }

  // Manejador de acciones
  handleActionClick(action: (item: any) => void, item: any) {
    // Emitir el evento para que el componente padre lo gestione
    this.actionClick.emit({ action, item });
  }

  // Manejador de paginación
  changePage(page: number) {
    if (page < 1 || page > this.totalPages) return;
    this.pageChange.emit(page);
  }

  // Función para generar un array de números para la paginación
  getPagesArray(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }
}
