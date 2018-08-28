import { CreateOrEditPolicy } from './../models/CreateOrEditPolicy';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '@env-ar/environment';
import { Observable } from 'rxjs';
import { PagedResult, DownloadService } from '@common/index';
import * as _ from 'lodash';
import { OverviewPolicy } from '@app-ar/policies/models/OverviewPolicy';
import { Policy } from '@app-ar/policies/models/Policy';

@Injectable({
  providedIn: 'root'
})
export class PoliciesApiService {
  baseUrl: string;

  constructor(private http: HttpClient, private fileDownload: DownloadService) {
    this.baseUrl = environment.api + 'delegations';
  }

  getAll(page: number, pageSize: number): Observable<PagedResult<OverviewPolicy>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    return this.http.get<PagedResult<OverviewPolicy>>(this.baseUrl, { params });
  }

  get(id: string): Observable<Policy> {
    return this.http.get<Policy>(`${this.baseUrl}/${id}`);
  }

  create(policy: CreateOrEditPolicy): Observable<Policy> {
    return this.http.post<Policy>(this.baseUrl, policy);
  }

  copy(policy: CreateOrEditPolicy): Observable<Policy> {
    return this.http.post<Policy>(this.baseUrl, _.extend(policy, { isCopy: true }));
  }

  update(id: string, policy: CreateOrEditPolicy): Observable<Policy> {
    return this.http.put<Policy>(`${this.baseUrl}/${id}`, policy);
  }

  delete(id: string): Observable<object> {
    return this.http.delete(`${this.baseUrl}/${id}`);
  }

  download(id: string): void {
    this.fileDownload.get(`${this.baseUrl}/json/${id}`).subscribe(
      response => {
        this.fileDownload.performBrowserDownload(response.filename, response.blob);
      },
      err => console.error('Cannot download. Error', err)
    );
  }

  get history() {
    return {
      download: (id: string): void => {
        this.fileDownload.get(`${this.baseUrl}/history/json/${id}`).subscribe(
          response => {
            this.fileDownload.performBrowserDownload(response.filename, response.blob);
          },
          err => console.error('Cannot download. Error', err)
        );
      }
    };
  }
}
