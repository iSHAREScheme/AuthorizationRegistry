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
import { AuthService } from '@common/generic/services/auth.service';
import * as _ from 'lodash';

@Injectable()
export class HttpErrorInterceptor implements HttpInterceptor {
  constructor(private router: Router, private alert: AlertService, private auth: AuthService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((response, caught) => {
        if (response instanceof HttpErrorResponse) {
          const result = new ResponseErrorModel();
          result.status = response.status;
          result.data = response.error;

          if (response.status === 401) {
            this.auth.logout();
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
            if (!Array.isArray(response.error)) {
              result.data = _.find(result.data);
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
