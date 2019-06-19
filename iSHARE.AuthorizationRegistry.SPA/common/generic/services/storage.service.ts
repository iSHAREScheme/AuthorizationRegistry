import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root'
})
export class StorageService {
  constructor(private cookieService: CookieService) {}
  getItem(key: string) {
    return this.cookieService.get(key);
  }
  setItem(key: string, value: string) {
    this.cookieService.set(key, value, null, '/');
  }
  removeItem(key: string) {
    this.cookieService.delete(key, '/');
  }
}
