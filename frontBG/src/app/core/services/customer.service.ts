import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { Customer } from '../models/customer.model';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {
  private readonly API_URL = environment.apiUrl;

  constructor(private http: HttpClient) { }


  getPagedCustomer(page: number, pageSize: number, searchTerm: string): Observable<any> {
    return this.http.get<any>(this.API_URL + `/Customer/paged?Page=${page}&PageSize=${pageSize}&SearchTerm=${searchTerm}`);
  }

  getCustomerByDocument(documentNumber: string): Observable<any> {
    return this.http.get<any>(this.API_URL + `/Customer/documentNumber/${documentNumber}`);
  }

  update(id: number, body: Partial<Customer>) {
    return this.http.put(this.API_URL + `/Customer/${id}`, body);
  }

  create(body: Partial<Customer>) {
    return this.http.post(this.API_URL + '/Customer', body);
  }

  delete(id: number) {
    return this.http.delete(this.API_URL + `/Customer/${id}`);
  }

}
