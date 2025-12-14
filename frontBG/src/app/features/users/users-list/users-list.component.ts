import { Component } from '@angular/core';
import { RegisterComponent } from "../../auth/register/register.component";
import { GenericTableComponent } from "../../../shared/components/generic-table/generic-table.component";
import { UserService } from '../../../core/services/user.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TokenService } from '../../../core/services/token.service';

@Component({
  selector: 'app-users-list',
  imports: [RegisterComponent, GenericTableComponent, FormsModule, CommonModule],
  templateUrl: './users-list.component.html',
  styleUrl: './users-list.component.css'
})
export class UsersListComponent {

  // Búsqueda y paginación
  searchTerm: string = '';
  page: number = 1;
  totalPages: number = 1;

  // Datos
  users: any[] = [];
  rolesSession: string[] = [];


  // Tabla genérica
  userColumns: any[] = [];
  userActions: any[] = [];

  // Modales
  selectedUser: any = null;
  showEditUser: boolean = false;
  showCreateUser: boolean = false;

  constructor(private userService: UserService, private tokenService: TokenService) {
    this.defineTableStructure();
    this.loadUsers();
  }

  ngOnInit(): void {
    this.rolesSession = this.tokenService.getUserRoles()
    console.log(this.rolesSession);
  }

  // Definición de columnas y acciones
  defineTableStructure() {
    this.userColumns = [
      { key: 'username', label: 'Usuario', textAlign: 'left' },
      { key: 'email', label: 'Email', textAlign: 'left' },
      { key: 'fullName', label: 'Nombre Completo', textAlign: 'left' },
      {
        key: 'roles',
        label: 'Roles',
        type: 'array',
        separator: ', ',
        textAlign: 'center'
      },
      {
        key: 'isActive',
        label: 'Estado',
        type: 'isActive',
        textAlign: 'center'
      }
    ];

    this.userActions = [
      {
        label: 'Editar',
        icon: 'bi-pencil',
        class: 'btn-secondary',
        action: (user: any) => this.openEdit(user)
      }
    ];
  }

  // Cargar usuarios (paginado)
  loadUsers() {
    const params = {
      Page: this.page.toString(),
      PageSize: '10',
      SearchTerm: this.searchTerm,
      activeOnly: 'false'
    };

    this.userService.getPagedUsers({ params })
      .subscribe((response: any) => {
        this.users = response.items;
        this.totalPages = response.totalPages;
      });
  }

  // Cambio de página
  handlePageChange(newPage: number) {
    this.page = newPage;
    this.loadUsers();
  }

  // Acción desde la tabla
  handleActionClick(event: { action: (item: any) => void, item: any }) {
    event.action(event.item);
  }

  // Abrir edición
  openEdit(user: any) {
    this.selectedUser = user;
    this.showEditUser = true;
  }

  // Crear usuario
  openCreateUser() {
    this.showCreateUser = true;
  }

  // createUser(body: any) {
  //   this.userService.create(body).subscribe(() => {
  //     this.showCreateUser = false;
  //     this.loadUsers();
  //   });
  // }

  // Actualizar usuario
  updateUser(event: { id: number; body: any }) {
    this.userService.update(event.id, event.body).subscribe(() => {
      this.showEditUser = false;
      this.loadUsers();
    });
  }

}
