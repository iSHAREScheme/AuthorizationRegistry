import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs/internal/Observable';
import { EnvironmentModel } from '../models/EnvironmentModel';
@Injectable()
export class SelfAuthGuard implements CanActivate {
    constructor(private environment: EnvironmentModel) {}
    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | Observable<boolean> | Promise < boolean > {
        return this.environment.userManagement;
    }
}
