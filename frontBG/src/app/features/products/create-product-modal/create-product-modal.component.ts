import { Component, EventEmitter, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-create-product-modal',
  imports: [FormsModule,],
  templateUrl: './create-product-modal.component.html',
  styleUrl: './create-product-modal.component.css'
})
export class CreateProductModalComponent {

  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<any>();

  model = {
    code: '',
    name: '',
    description: '',
    unitPrice: 0,
    stock: 0,
    minStock: 0
  };

  onSave() {
    this.save.emit(this.model);
  }
}
