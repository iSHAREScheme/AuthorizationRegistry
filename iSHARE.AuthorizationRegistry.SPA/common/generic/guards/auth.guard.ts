import { Injectable, Inject, OnDestroy } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs/internal/Observable';
import { AuthService } from '../services/auth.service';
import { constants } from '../../constants';
import { LandingPage } from '../services/landing-page';

@Injectable()
export class AuthGuard implements CanActivate, OnDestroy {
  authKey: string;
  constructor(private authService: AuthService, private landingPage: LandingPage) {
    this.authKey = constants.storage.keys.auth;
    window.addEventListener('storage', this.storageEventListener.bind(this), false);
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    if (this.authService.isLoggedIn()) {
      return true;
    }
    this.landingPage.navigate();
    // this.authService.goToLogin();
    return false;
  }

  storageEventListener(event: StorageEvent) {
    if (event.storageArea === localStorage && event.key === this.authKey) {
      window.location.reload();
    }
  }

  ngOnDestroy(): void {
    window.removeEventListener('storage', this.storageEventListener, false);
  }
}
