import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-edit-product-modal',
  imports: [CommonModule, FormsModule],
  templateUrl: './edit-product-modal.component.html',
  styleUrl: './edit-product-modal.component.css'
})
export class EditProductModalComponent {
  @Input() product: any | null = null;

  @Output() close = new EventEmitter<void>();
  @Output() update = new EventEmitter<{ id: number; body: any }>();

  model = {
    name: '',
    description: '',
    unitPrice: 0,
    stock: 0,
    minStock: 0,
    isActive: true
  };

  ngOnChanges(): void {
    if (this.product) {
      this.model = {
        name: this.product.name,
        description: this.product.description,
        unitPrice: this.product.unitPrice,
        stock: this.product.stock,
        minStock: this.product.minStock,
        isActive: this.product.isActive
      };
    }
  }

  onUpdate() {
    this.update.emit({
      id: this.product.id,
      body: this.model
    });
  }

}
