import { Component, EventEmitter, Input, Output } from '@angular/core';
import { UpdateUserRequest } from '../../../core/models/user.model';
import { UserService } from '../../../core/services/user.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-edit-user-modal',
  imports: [FormsModule, CommonModule],
  templateUrl: './edit-user-modal.component.html',
  styleUrl: './edit-user-modal.component.css'
})
export class EditUserModalComponent {
  @Input() userId!: number;
  @Input() model!: UpdateUserRequest;

  @Output() close = new EventEmitter<void>();
  @Output() updated = new EventEmitter<void>();

  constructor(private usersService: UserService) { }

  onUpdate() {
    this.usersService.update(this.userId, this.model)
      .subscribe({
        next: () => {
          this.updated.emit();
          this.close.emit();
        },
        error: () => {
          alert('Error al actualizar el usuario');
        }
      });
  }
}
