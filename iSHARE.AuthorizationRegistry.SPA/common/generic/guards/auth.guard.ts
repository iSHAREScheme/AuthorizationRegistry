import { Injectable, Inject, OnDestroy } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs/internal/Observable';
import { AuthService } from '../services/auth.service';
import { LandingPage } from '../services/landing-page';

@Injectable()
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private landingPage: LandingPage) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
    if (this.authService.isLoggedIn()) {
      return true;
    }
    this.landingPage.navigate();
    return false;
  }
}
