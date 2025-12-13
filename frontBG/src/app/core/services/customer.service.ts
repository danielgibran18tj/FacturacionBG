import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {
  private readonly API_URL = environment.apiUrl;

  constructor(private http: HttpClient) { }


  getPagedCustomer(page: number, pageSize: number, searchTerm: string): Observable<any> {
    return this.http.get<any>(this.API_URL + `/customer/paged?Page=${page}&PageSize=${pageSize}&SearchTerm=${searchTerm}`);
  }

  getCustomerByDocument(documentNumber: string): Observable<any> {
    return this.http.get<any>(this.API_URL + `/Customer/documentNumber/${documentNumber}`);
  }

}
