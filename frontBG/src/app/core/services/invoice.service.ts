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
    return this.http.post<any>(this.API_URL + "/Invoice/paged", body);
  }

  downloadInvoicePdf(id: number) {
    return this.http.get(this.API_URL + `/Invoice/${id}/pdf`, {
      responseType: 'blob'
    });
  }

  delete(id: number) {
    return this.http.delete(`/api/Invoice/${id}`);
  }

  saveInvoices(body: any): Observable<any> {
    return this.http.post<any>(this.API_URL + "/Invoice", body);
  }
}
