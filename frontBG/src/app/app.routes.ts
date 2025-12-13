import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { AuthGuard } from './core/guards/auth.guard';
import { RoleGuard } from './core/guards/role.guard';
import { NoAuthorizationComponent } from './features/no-authorization/no-authorization/no-authorization.component';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'unauthorized', component: NoAuthorizationComponent },
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [AuthGuard],
    children: [ // aplicar LazyLoading
      {
        path: '',
        redirectTo: 'invoices',
        pathMatch: 'full'
      },
      {
        // de esta manera traera el archivo javascript solo cuando se vaya a usar por el usuario
        path: 'invoices',
        loadComponent: () => import('./features/invoices/facturas-list/facturas-list.component').then(m => m.FacturasListComponent)
      },
      {
        path: 'products',
        loadComponent: () => import('./features/products/products-list/products-list.component').then(m => m.ProductsListComponent),
        data: { roles: ['Administrator', 'Seller'] },
        canActivate: [RoleGuard]
      },
      {
        path: 'customers',
        loadComponent: () => import('./features/customers/customers-list/customers-list.component').then(m => m.CustomersListComponent),
        data: { roles: ['Administrator', 'Seller'] },
        canActivate: [RoleGuard]
      },
      {
        path: 'system-settings',
        loadComponent: () => import('./features/system-settings/system-settings/system-settings.component').then(m => m.SystemSettingsComponent),
        data: { roles: ['Administrator'] },
        canActivate: [RoleGuard]
      },
    ]
  },
  { path: '**', redirectTo: '/login' }
];
