import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { CreateDelegationMask } from './../models/CreateDelegationMask';
import { environment } from '@env-ar/environment';

@Injectable({
  providedIn: 'root'
})
export class MaskApiService {
  baseUrl: string;

  constructor(private http: HttpClient) {
    this.baseUrl = environment.api + 'delegation/test';
  }

  test(mask: CreateDelegationMask): Observable<CreateDelegationMask> {
    return this.http.post<CreateDelegationMask>(this.baseUrl, mask);
  }
}
