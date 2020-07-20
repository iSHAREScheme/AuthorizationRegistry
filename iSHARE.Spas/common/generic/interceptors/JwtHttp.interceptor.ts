import { Injectable, Inject } from '@angular/core';
import { HttpInterceptor, HttpHandler, HttpEvent, HttpRequest, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Observable } from 'rxjs/internal/Observable';
import { StorageKeysModel } from '../models/StorageKeysModel';
import { StorageService } from '../services/storage.service';

@Injectable()
export class JwtHttpInterceptor implements HttpInterceptor {
  constructor(private storageKeys: StorageKeysModel, private storageService: StorageService) {}
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = this.storageService.getItem(this.storageKeys.auth);
    let clone: HttpRequest<any>;
    if (token) {
      clone = request.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
    } else {
      clone = request.clone();
    }
    return next.handle(clone);
  }
}

export const JwtHttpInterceptorProvider = {
  provide: HTTP_INTERCEPTORS,
  useClass: JwtHttpInterceptor,
  multi: true
};
