import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  private readonly API_URL = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getPagedProducts(body: any): Observable<any> {
    return this.http.get<any>(this.API_URL + "/Product/paged", body);
  }

  create(body: any) {
    return this.http.post(this.API_URL + '/Product', body);
  }

  update(id: number, body: any) {
    return this.http.put(this.API_URL + `/Product/${id}`, body);
  }

}
