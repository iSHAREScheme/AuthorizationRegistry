import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs/internal/Observable';
import { AppInsightsService } from '../services/app-insights.service';

@Injectable()
export class AppInsightsInterceptor implements CanActivate {
  constructor(private service: AppInsightsService) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
    this.service.logPageView(state.url);
    this.service.logEvent(state.url);
    return true;
  }
}
