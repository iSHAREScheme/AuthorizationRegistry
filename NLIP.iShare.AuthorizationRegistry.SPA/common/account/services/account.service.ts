import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { EnvironmentModel, ResponseModel } from '@generic/index';

import { ChangePasswordModel } from '../models/ChangePasswordModel';
import { ActivateAccountModel } from '../models/ActivateAccountModel';
import { ForgotPasswordModel } from '../models/ForgotPasswordModel';
import { ResetPasswordModel } from '../models/ResetPasswordModel';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  environment: EnvironmentModel;
  baseUrl: string;

  constructor(
    private http: HttpClient,
    @Inject('environmentProvider') private environmentProvider: EnvironmentModel
  ) {
    this.environment = environmentProvider;
    this.baseUrl = this.environment.api + 'account';
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
}
