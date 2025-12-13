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
    console.log(body);
    
    return this.http.get<any>(this.API_URL + "/Product/paged", body);
  }


}
