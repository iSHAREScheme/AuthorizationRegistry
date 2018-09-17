import { Injectable, Inject } from '@angular/core';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, ReplaySubject } from 'rxjs';
import * as _ from 'lodash';
import { ProfileService } from '../services/profile.service';
import { EnvironmentModel } from '../models/EnvironmentModel';

@Injectable()
export class AuthService {
  private currentToken: string;
  loginUrl: string;
  environment: any;

  constructor(
    private http: HttpClient,
    private jwtHelperService: JwtHelperService,
    private router: Router,
    private profileService: ProfileService,
    @Inject('environmentProvider') private environmentProvider: EnvironmentModel
  ) {
    this.environment = environmentProvider;
    this.loginUrl = `${this.environment.api}connect/token`;
  }

  login(username, password): Promise<any> {
    this.logout();
    const promise = new Promise<any>((resolve, reject) => {
      localStorage.removeItem(this.environment.localStorageKeys.auth);

      const body = new HttpParams()
        .set('grant_type', 'password')
        .set('client_id', 'SPA')
        .set('client_secret', 'secret')
        .set('scope', this.environment.scope)
        .set('username', username)
        .set('password', password);

      this.http
        .post(this.loginUrl, body.toString(), {
          headers: new HttpHeaders().set('Content-Type', 'application/x-www-form-urlencoded')
        })
        .toPromise()
        .then(
          response => {
            this.currentToken = (<any>response).access_token;
            localStorage.setItem(this.environment.localStorageKeys.auth, this.currentToken);
            const tokenExpiringDate = Date.now() + (<any>response).expires_in * 1000;
            localStorage.setItem(this.environment.localStorageKeys.authExpiration, JSON.stringify(tokenExpiringDate));
            const decodedToken = this.jwtHelperService.decodeToken(this.currentToken);
            this.profileService.set({
              email: decodedToken.email,
              partyId: decodedToken.partyId,
              partyName: decodedToken.partyName,
              role: decodedToken.role,
              id: decodedToken.sub
            });
            resolve();
          },
          err => reject(err)
        );
    });

    return promise;
  }

  logout(): void {
    this.clearLogout();
    this.router.navigate(['account/login']);
  }
  clearLogout(): void {
    this.currentToken = null;
    localStorage.removeItem(this.environment.localStorageKeys.auth);
    localStorage.removeItem(this.environment.localStorageKeys.authExpiration);
    this.profileService.clear();
  }

  isLoggedIn(): boolean {
    const expiringDate = JSON.parse(localStorage.getItem(this.environment.localStorageKeys.authExpiration));
    if (expiringDate && Date.now() > expiringDate) {
      return false;
    }
    if (!this.currentToken) {
      this.currentToken = localStorage.getItem(this.environment.localStorageKeys.auth);
    }
    return !!this.currentToken;
  }

  authorize(roles: string[]): Observable<boolean> {
    const subject = new ReplaySubject<boolean>(1);
    const profile = this.profileService.get();

    const authorized =
      typeof profile.role === 'string'
        ? _.some(roles, role => profile.role === role)
        : _.some(roles, role => _.some(profile.role, profileRole => profileRole === role));

    subject.next(authorized);
    subject.complete();
    return subject.asObservable();
  }
}
