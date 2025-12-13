import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { SystemSetting } from '../models/systemSetting.model';

@Injectable({
  providedIn: 'root'
})
export class SystemSettingsService {

  private readonly API_URL = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getAll() {
    return this.http.get<SystemSetting[]>(this.API_URL + '/system-settings');
  }

  getIva() {
    return this.http.get<number>(this.API_URL + '/system-settings/iva');
  }

  updateSetting(key: string, value: string) {
    const body = {
      value: value.toString()
    };
    return this.http.put(this.API_URL + `/system-settings/${key}`, body);
  }

}
