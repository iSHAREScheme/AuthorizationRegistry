import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs/internal/Observable';
import { EnvironmentModel, Query, PagedResult } from '@common/generic';
import { User } from '../models/User';

@Injectable({
  providedIn: 'root'
})
export class UsersApiService {
  baseUrl: string;

  constructor(private http: HttpClient, private environment: EnvironmentModel) {
    this.baseUrl = `${this.environment.apiEndpoint}/users`;
  }

  getAll(query: Query) {
    const params = new HttpParams()
      .set('filter', query.filter)
      .set('page', query.page.toString())
      .set('pageSize', query.pageSize.toString())
      .set('sortBy', query.sortBy)
      .set('sortOrder', query.sortOrder);
    return this.http.get<PagedResult<User>>(this.baseUrl, { params });
  }

  create(user: Partial<User>): Observable<User> {
    return this.http.post<User>(this.baseUrl, user);
  }

  get(id: string) {
    return this.http.get<User>(`${this.baseUrl}/${id}`);
  }

  sendActivate(user: Partial<User>): Observable<User> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });
    const link = this.baseUrl + '/activate';
    return this.http.post<User>(link, user, { headers });
  }

  resetPassword(user: Partial<User>) {
    const url = `${this.baseUrl}/${user.id}/password`;
    return this.http.post(url, null);
  }

  update(user: Partial<User>) {
    return this.http.put<User>(`${this.baseUrl}/${user.id}`, user);
  }

  delete(id: string) {
    return this.http.delete(`${this.baseUrl}/${id}`);
  }
}
