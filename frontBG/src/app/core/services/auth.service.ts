import { Injectable } from '@angular/core';
import { User } from '../models/user.model';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { ApiResponse, LoginRequest, LoginResponse, RegisterRequest } from '../models/auth.model';
import { TokenService } from './token.service';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly API_URL = environment.apiUrl;
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(
    private http: HttpClient,
    private tokenService: TokenService,
    private router: Router
  ) {
    // Cargar usuario del localStorage si existe
    const user = this.tokenService.getUser();
    if (user) {
      this.currentUserSubject.next(user);
    }
  }

  login(credentials: LoginRequest): Observable<ApiResponse<LoginResponse>> {
    return this.http.post<ApiResponse<LoginResponse>>(
      `${this.API_URL}/auth/login`,
      credentials
    ).pipe(
      tap(response => {
        if (response.success && response.data) {
          this.tokenService.saveTokens(
            response.data.accessToken,
            response.data.refreshToken
          );
          this.tokenService.saveUser(response.data.user);
          this.currentUserSubject.next(response.data.user);
        }
      })
    );
  }

  register(data: RegisterRequest): Observable<ApiResponse<User>> {
    return this.http.post<ApiResponse<User>>(
      `${this.API_URL}/auth/register`,
      data
    );
  }

  logout(): void {
    const refreshToken = this.tokenService.getRefreshToken();
    
    if (refreshToken) {
      this.http.post(`${this.API_URL}/auth/logout`, { refreshToken })
        .subscribe({
          complete: () => this.clearSession()
        });
    } else {
      this.clearSession();
    }
  }

  refreshToken(): Observable<ApiResponse<LoginResponse>> {
    const refreshToken = this.tokenService.getRefreshToken();
    return this.http.post<ApiResponse<LoginResponse>>(
      `${this.API_URL}/auth/refresh-token`,
      { refreshToken }
    ).pipe(
      tap(response => {
        if (response.success && response.data) {
          this.tokenService.saveTokens(
            response.data.accessToken,
            response.data.refreshToken
          );
        }
      })
    );
  }
  
  getDataUserSession() {
    return this.http.get(this.API_URL + '/Auth/me');
  }

  isAuthenticated(): boolean {
    return !!this.tokenService.getAccessToken() && 
           !this.tokenService.isTokenExpired();
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  private clearSession(): void {
    this.tokenService.clearTokens();
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }
}

