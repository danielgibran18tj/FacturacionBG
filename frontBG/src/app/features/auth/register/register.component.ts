import { Component, EventEmitter, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { TokenService } from '../../../core/services/token.service';


@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, CommonModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  registerForm!: FormGroup;
  loading = false;
  showPassword = false;
  showConfirmPassword = false;
  errorMessage = '';
  @Output() close = new EventEmitter<void>();
  roles: string[] = [];
  isAdmin = false;
  availableRoles: string[] = [
    'Administrator',
    'Seller',
    'Customer'
  ];

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private tokenService: TokenService,
    private router: Router
  ) { }

  ngOnInit(): void {
    if (!this.authService.isAuthenticated()) {
      this.tokenService.clearTokens();
    }

    this.registerForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      username: ['', [
        Validators.required,
        Validators.minLength(3),
        Validators.pattern(/^[a-zA-Z0-9_-]+$/)
      ]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [
        Validators.required,
        Validators.minLength(8),
        Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d]/)
      ]],
      confirmPassword: ['', Validators.required],
      roles: [[]]
    }, { validators: this.passwordMatchValidator });

    this.roles = this.tokenService.getUserRoles()
    if (!this.roles.includes('Administrator')) {
      this.isAdmin = false;
      this.registerForm.get('roles')?.setValue(['Customer']);
    } else this.isAdmin = true;
  }

  passwordMatchValidator(control: AbstractControl): { [key: string]: boolean } | null {
    const password = control.get('password');
    const confirmPassword = control.get('confirmPassword');

    if (!password || !confirmPassword) {
      return null;
    }

    return password.value === confirmPassword.value ? null : { passwordMismatch: true };
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      Object.keys(this.registerForm.controls).forEach(key => {
        this.registerForm.get(key)?.markAsTouched();
      });
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    const { confirmPassword, ...registerData } = this.registerForm.value;

    this.authService.register(registerData).subscribe({
      next: (response) => {
        if (response.success) {
          this.router.navigate(['/login'], {
            queryParams: { registered: 'true' }
          });
        }
      },
      error: (error) => {
        this.loading = false;

        if (error.error?.message) {
          this.errorMessage = error.error.message;
        } else if (error.error?.errors?.length > 0) {
          this.errorMessage = '<ul class="mb-0 ps-3">' +
            error.error.errors.map((e: any) => `<li>${e.message}</li>`).join('') +
            '</ul>';
        } else {
          this.errorMessage = 'Error al registrar usuario. Intenta nuevamente.';
        }
      },
      complete: () => {
        this.loading = false;
      }
    });
  }

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  toggleConfirmPassword(): void {
    this.showConfirmPassword = !this.showConfirmPassword;
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.registerForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  hasAnyRole(roles: string[]): boolean {
    return roles.some(r => this.roles.includes(r));
  }

  hasRole(role: string): boolean {
    return this.roles.includes(role);
  }

  onRoleChange(role: string, event: any) {
    const roles = this.registerForm.get('roles')?.value as string[];

    if (event.target.checked) {
      this.registerForm.get('roles')?.setValue([...roles, role]);
    } else {
      this.registerForm.get('roles')?.setValue(
        roles.filter(r => r !== role)
      );
    }
  }

}
