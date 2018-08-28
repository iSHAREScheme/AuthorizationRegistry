import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpInterceptor,
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HTTP_INTERCEPTORS
} from '@angular/common/http';
import { Router } from '@angular/router';
import { catchError } from 'rxjs/operators';
import { Observable, throwError } from 'rxjs';
import { AlertService } from '../services/alert.service';

@Injectable()
export class HttpErrorInterceptor implements HttpInterceptor {
  constructor(private router: Router, private alert: AlertService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((response, caught) => {
        if (response instanceof HttpErrorResponse) {
          const result = new ResponseErrorModel();
          result.status = response.status;
          result.data = response.error;

          if (response.status === 401) {
            this.router.navigate(['account/login']);
          } else if (response.status === 404) {
            result.message = 'Inexistent resource.';
            result.alert = this.alert.error(result.message);
          } else if (response.status >= 400) {
          } else if (response.status >= 500) {
            result.message = 'Server error. Please contact administrator.';
            result.alert = this.alert.error(result.message);
          }

          if (response.error && typeof response.error === 'object') {
            if (response.error.length > 0) {
              result.message = response.error.reduce((a, i) => a + i + '\r\n');
            } else {
              result.message = Object.keys(response.error).reduce(
                (a, i) => a + response.error[i] + '\r\n'
              );
            }
            if (typeof response.error.errors === 'object') {
              result.data = response.error.errors;
            }
          }

          return throwError(result);
        }
      })
    );
  }
}

export const ErrorInterceptorProvider = {
  provide: HTTP_INTERCEPTORS,
  useClass: HttpErrorInterceptor,
  multi: true
};
export class ResponseErrorModel {
  alert: any;
  data: any;
  message: string;
  status: number;
  errors: string[];
}
