import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  loginForm!: FormGroup;
  loading = false;
  showPassword = false;
  errorMessage = '';
  successMessage = '';
  returnUrl = '/dashboard';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    // Si ya está autenticado, redirigir
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/dashboard']);
      return;
    }

    this.loginForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });

    // Obtener URL de retorno
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/dashboard';

    // Mensaje de registro exitoso
    if (this.route.snapshot.queryParams['registered'] === 'true') {
      this.successMessage = '¡Registro exitoso! Ahora puedes iniciar sesión';
    }
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      Object.keys(this.loginForm.controls).forEach(key => {
        this.loginForm.get(key)?.markAsTouched();
      });
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    this.authService.login(this.loginForm.value).subscribe({
      next: (response) => {
        if (response.success) {
          this.successMessage = '¡Inicio de sesión exitoso!';
          setTimeout(() => {
            this.router.navigate([this.returnUrl]);
          }, 500);
        }
      },
      error: (error) => {
        this.loading = false;
        
        if (error.error?.message) {
          this.errorMessage = error.error.message;
        } else if (error.error?.errors?.length > 0) {
          this.errorMessage = error.error.errors.map((e: any) => e.message).join(', ');
        } else {
          this.errorMessage = 'Error al iniciar sesión. Verifica tus credenciales.';
        }

        console.error('Login error:', error);
      },
      complete: () => {
        this.loading = false;
      }
    });
  }

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.loginForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }
}
