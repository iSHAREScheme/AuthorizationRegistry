import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs/internal/Observable';
import { AppInsightsService } from '../services/app-insights.service';
import { ProfileService } from '@common/generic/services/profile.service';

@Injectable()
export class AppInsightsInterceptor implements CanActivate {
  constructor(
    private service: AppInsightsService,
    private profileService: ProfileService
    ) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {

    const profile = this.profileService.get();
    const properties: any = {};
    if (profile) {
      properties.userId = profile.id;
      if (profile.partyId) {
        properties.partyId = profile.partyId;
      }
    }

    this.service.logPageView(undefined, state.url, properties, undefined, undefined);
    return true;
  }
}
