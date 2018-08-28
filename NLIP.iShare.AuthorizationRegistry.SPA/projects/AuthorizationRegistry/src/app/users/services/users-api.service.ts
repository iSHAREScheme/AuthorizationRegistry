import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { environment } from '@env-ar/environment';
import { User } from '@app-ar/users/models/User';
import { PagedResult } from '@common/index';
import { Observable } from 'rxjs/internal/Observable';

@Injectable({
  providedIn: 'root'
})
export class UsersApiService {
  baseUrl: string;

  constructor(private http: HttpClient) {
    this.baseUrl = environment.api + 'users';
  }

  getAll(page: number, pageSize: number, sortBy: string, sortOrder: 'asc' | 'desc'): Observable<PagedResult<User>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString())
      .set('sortBy', sortBy)
      .set('sortOrder', sortOrder);
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
