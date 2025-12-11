import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { AuthGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [AuthGuard],
    children: [ // aplicar LazyLoading
      {
        path: '',
        redirectTo: 'facturas',
        pathMatch: 'full'
      },
      {
        // de esta manera traera el archivo javascript solo cuando se vaya a usar por el usuario
        path: 'facturas',
        loadComponent: () => import('./features/invoices/facturas-list/facturas-list.component').then(m => m.FacturasListComponent)
      },
    ]
  },
  { path: '**', redirectTo: '/login' }
];
