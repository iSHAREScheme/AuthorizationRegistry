import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, ReplaySubject } from 'rxjs';
import * as _ from 'lodash';
import { ProfileService } from '../services/profile.service';
import { constants } from '../../constants';
import { AuthServiceOptions } from '../models/AuthServiceOptions';
import { EnvironmentModel } from '../models/EnvironmentModel';

@Injectable()
export class AuthService {
  private options: AuthServiceOptions;
  constructor(
    private http: HttpClient,
    private jwtHelperService: JwtHelperService,
    private router: Router,
    private profileService: ProfileService,
    private config: EnvironmentModel
  ) {
    this.options = new AuthServiceOptions(this.config);
  }

  getAuthorizationUrl(): string {
    const params = {
      client_id: this.options.clientId,
      redirect_uri: this.options.redirectEndpoint,
      response_type: 'token',
      scope: this.options.scope
    };
    const query = Object.keys(params)
      .map(key => `${key}=${encodeURIComponent(params[key])}`)
      .join('&');
    const result = this.options.authorizeEndpoint + '?' + query;
    return result;
  }

  goToLogin() {
    window.location.href = this.getAuthorizationUrl();
  }
  login(username, password) {
    const body = new HttpParams().set('username', username).set('password', password);
    return this.http.post<any>(this.options.loginEndpoint, body.toString(), {
      headers: new HttpHeaders().set('Content-Type', 'application/x-www-form-urlencoded'),
      withCredentials: true
    });
  }

  loginWith2Fa(username, password, code) {
    const body = new HttpParams()
      .set('username', username)
      .set('password', password)
      .set('twoFactorCode', code);
    return this.http.post<any>(this.options.loginEndpoint, body.toString(), {
      headers: new HttpHeaders().set('Content-Type', 'application/x-www-form-urlencoded'),
      withCredentials: true
    });
  }

  setAccessToken(accessToken: string, expirationTime: string) {
    const decodedToken = this.jwtHelperService.decodeToken(accessToken);
    this.profileService.set({
      email: decodedToken.email,
      partyId: decodedToken.partyId,
      partyName: decodedToken.partyName,
      role: decodedToken.role,
      id: decodedToken.sub
    });
    localStorage.setItem(constants.storage.keys.auth, accessToken);
    const tokenExpiringDate = Date.now() + <any>expirationTime * 1000;
    localStorage.setItem(constants.storage.keys.authExpiration, JSON.stringify(tokenExpiringDate));
    this.router.navigate(['']);
  }

  logout(): void {
    this.clearLogout();
    this.http.post<any>(this.options.logoutEndpoint, null, { withCredentials: true }).subscribe(() => {
      this.goToLogin();
    });
  }
  clearLogout(): void {
    this.profileService.clear();
    localStorage.removeItem(constants.storage.keys.authExpiration);
    localStorage.removeItem(constants.storage.keys.auth);
  }

  isLoggedIn(): boolean {
    const expiringDate = JSON.parse(localStorage.getItem(constants.storage.keys.authExpiration));
    if (expiringDate && Date.now() > expiringDate) {
      return false;
    }
    return !!this.getToken();
  }

  getToken() {
    return localStorage.getItem(constants.storage.keys.auth);
  }

  authorize(roles: string[]): Observable<boolean> {
    const subject = new ReplaySubject<boolean>(1);
    subject.next(this.isInRole(roles));
    subject.complete();
    return subject.asObservable();
  }

  inRole(roles: string[]): boolean {
    return this.isInRole(roles);
  }

  private isInRole(roles: string[]): boolean {
    const profile = this.profileService.get();

    if (profile == null) {
      return false;
    }

    if (roles == null || roles.length === 0) {
      return true; // return true if no particular role is requested
    }

    const hasRole =
      typeof profile.role === 'string'
        ? _.some(roles, role => profile.role === role)
        : _.some(roles, role => _.some(profile.role, profileRole => profileRole === role));

    return hasRole;
  }
}
