import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class InvoiceService {
  private readonly API_URL = environment.apiUrl;

  constructor(private http: HttpClient) { }


   getPagedInvoices(body: any): Observable<any> {
    console.log(this.API_URL);
    
    return this.http.post<any>(this.API_URL + "/Invoice/paged" , body);
  }


}
