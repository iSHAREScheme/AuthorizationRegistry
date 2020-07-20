import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { EnvironmentModel, CreateDelegationMask } from 'common';

@Injectable({
  providedIn: 'root'
})
export class MaskApiService {
  baseUrl: string;

  constructor(private http: HttpClient, private environment: EnvironmentModel) {
    this.baseUrl = `${this.environment.apiEndpoint}/delegations/test`;
  }

  test(mask: CreateDelegationMask): Observable<CreateDelegationMask> {
    return this.http.post<CreateDelegationMask>(this.baseUrl, mask);
  }
}
