import { Injectable, Inject } from '@angular/core';
import { HttpInterceptor, HttpHandler, HttpEvent, HttpRequest, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Observable } from 'rxjs/internal/Observable';
import { constants } from '../../constants';

@Injectable()
export class JwtHttpInterceptor implements HttpInterceptor {
  constructor() {}
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = localStorage.getItem(constants.storage.keys.auth);
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
