import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PaymentMethodService {

  private readonly API_URL = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getPaymentMethod(): Observable<any> {
    return this.http.get<any>(this.API_URL + "/PaymentMethod");
  }
}
