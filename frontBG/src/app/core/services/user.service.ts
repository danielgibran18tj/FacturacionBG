import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { UpdateUserRequest } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private readonly API_URL = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getPagedUsers(options: any) {
    return this.http.get(this.API_URL + '/Users/paged', options);
  }

  update(id: number, body: UpdateUserRequest) {
    return this.http.put(this.API_URL + `/Users/${id}`, body);
  }

}
