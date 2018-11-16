import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { CreateDelegationMask } from './../models/CreateDelegationMask';
import { EnvironmentModel } from 'common';

@Injectable({
  providedIn: 'root'
})
export class MaskApiService {
  baseUrl: string;

  constructor(private http: HttpClient, private environment: EnvironmentModel) {
    this.baseUrl = `${this.environment.apiEndpoint}/delegation/test`;
  }

  test(mask: CreateDelegationMask): Observable<CreateDelegationMask> {
    return this.http.post<CreateDelegationMask>(this.baseUrl, mask);
  }
}
