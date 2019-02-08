import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable } from 'rxjs/internal/Observable';
import { AuthService } from '../services/auth.service';
import { ReplaySubject } from 'rxjs';

@Injectable()
export class RoleGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
    const allowedRoles = <string[]>route.data['allowedRoles'];
    const subject = new ReplaySubject<boolean>(1);
    this.authService.authorize(allowedRoles).subscribe(authorized => {
      subject.next(authorized);
      subject.complete();
      if (!authorized) {
        this.router.navigate(['forbidden']);
      }
    });
    return subject.asObservable();
  }
}
