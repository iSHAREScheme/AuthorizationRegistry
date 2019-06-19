import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, ReplaySubject } from 'rxjs';
import * as _ from 'lodash';
import { ProfileService } from '../services/profile.service';
import { AuthServiceOptions } from '../models/AuthServiceOptions';
import { EnvironmentModel } from '../models/EnvironmentModel';
import { StorageKeysModel } from '../models/StorageKeysModel';
import { StorageService } from './storage.service';

@Injectable()
export class AuthService {
  private options: AuthServiceOptions;
  constructor(
    private http: HttpClient,
    private jwtHelperService: JwtHelperService,
    private router: Router,
    private profileService: ProfileService,
    private config: EnvironmentModel,
    private storageKeys: StorageKeysModel,
    private storageService: StorageService
  ) {
    this.options = new AuthServiceOptions(this.config);
  }

  getAuthorizationUrl(): string {
    const params = {
      client_id: this.options.clientId,
      redirect_uri: this.options.redirectEndpoint,
      response_type: 'code id_token',
      scope: `${this.options.scope} openid profile`,
      nonce: this.generateNonce()
    };
    const query = Object.keys(params)
      .map(key => `${key}=${encodeURIComponent(params[key])}`)
      .join('&');
    const result = this.config.identityProvider.authorityUrl + 'connect/authorize' + '?' + query;
    return result;
  }

  generateNonce() {
    const length = 40;
    const chars = '0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ';
    let result = '';
    for (let i = length; i > 0; --i) {
      result += chars[Math.floor(Math.random() * chars.length)];
    }
    return result;
  }

  goToLogin() {
    window.location.href = this.getAuthorizationUrl();
  }

  goToLoginPage() {
    this.router.navigate(['account', 'login']);
  }
  login(username, password) {
    username = encodeURIComponent(username);
    password = encodeURIComponent(password);
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

  setAccessToken(accessToken: string, identityToken: string, expirationTime: string) {
    const decodedIdentityToken = this.jwtHelperService.decodeToken(identityToken);
    this.profileService.set({
      email: decodedIdentityToken.email,
      partyId: decodedIdentityToken.partyId,
      partyName: decodedIdentityToken.partyName,
      role: decodedIdentityToken.role,
      id: decodedIdentityToken.sub
    });
    this.storageService.setItem(this.storageKeys.auth, accessToken);
    const tokenExpiringDate = Date.now() + <any>expirationTime * 1000;
    this.storageService.setItem(this.storageKeys.authExpiration, JSON.stringify(tokenExpiringDate));
    this.triggerRefresh();
    this.router.navigate(['']);
  }

  logout(): void {
    this.http
      .post<any>(this.options.logoutEndpoint, null, { withCredentials: true })
      .subscribe(() => {
        this.clearLogout();
        this.triggerRefresh();
        this.goToLoginPage();
      });
  }
  triggerRefresh() {
    localStorage.setItem('refresh', 'true');
    localStorage.removeItem('refresh');
  }
  registerReload(host: any) {
    window.addEventListener('storage', this.storageEventListener.bind(host), false);
  }
  storageEventListener(event: StorageEvent) {
    if (event.storageArea === localStorage && event.key === 'refresh') {
      window.location.reload();
    }
  }

  clearLogout(): void {
    this.profileService.clear();
    this.storageService.removeItem(this.storageKeys.authExpiration);
    this.storageService.removeItem(this.storageKeys.auth);
  }

  isLoggedIn(): boolean {
    const expiringDate = JSON.parse(
      this.storageService.getItem(this.storageKeys.authExpiration) || '{}'
    );
    if (expiringDate && Date.now() > expiringDate) {
      return false;
    }
    return !!this.getToken();
  }

  getToken() {
    return this.storageService.getItem(this.storageKeys.auth);
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
