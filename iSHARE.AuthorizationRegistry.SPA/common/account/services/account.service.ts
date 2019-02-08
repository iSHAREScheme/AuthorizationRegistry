import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { ResponseModel, EnvironmentModel } from '@generic/index';

import { ChangePasswordModel } from '../models/ChangePasswordModel';
import { ActivateAccountModel } from '../models/ActivateAccountModel';
import { ForgotPasswordModel } from '../models/ForgotPasswordModel';
import { ResetPasswordModel } from '../models/ResetPasswordModel';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl: string;

  constructor(private http: HttpClient, private environment: EnvironmentModel) {
    this.baseUrl = `${this.environment.apiEndpoint}/account`;
  }

  changePassword(model: ChangePasswordModel) {
    return this.http.patch<ResponseModel>(`${this.baseUrl}/password`, model);
  }

  activate(model: ActivateAccountModel) {
    return this.http.post<ResponseModel>(`${this.baseUrl}/activate`, model);
  }

  sendForgotPasswordEmail(user: ForgotPasswordModel) {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });
    return this.http.post<ResponseModel>(`${this.baseUrl}/forgot-password`, user, { headers });
  }

  confirmPasswordReset(model: ResetPasswordModel) {
    return this.http.post<ResponseModel>(`${this.baseUrl}/reset-password`, model);
  }

  enable2fa(username: string, password: string, code: string) {
    return this.http.post<any>(`${this.baseUrl}/2fa/enable`, {
      username: username,
      password: password,
      code: code
    });
  }

  getAuthenticatorKey(username: string, password: string) {
    return this.http.post<any>(`${this.baseUrl}/2fa/key`, {
      username: username,
      password: password
    });
  }
}
