import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { AuthService, ProfileService, Profile } from '@common/index';
import { AppInsightsService } from 'common';
import { Router } from '@angular/router';
import { RuntimeConfigurationService, ConfigurationModel } from '@generic/services/runtime-configuration.service';
import { MENU_ITEMS } from './menu/menu-items';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  profile: Profile;
  menuItems = MENU_ITEMS;

  constructor(
    private auth: AuthService,
    private configurationService: RuntimeConfigurationService,
    private profileService: ProfileService,
    // Tracking is not properly enabled without injecting AppInsightsService
    private appInsightsService: AppInsightsService,
    private router: Router,
    private changeDetectorRef: ChangeDetectorRef
  ) {
    appInsights.queue.push(() => {
      appInsights.context.addTelemetryInitializer(envelope => {
        envelope.tags['ai.cloud.role'] = 'ar.spa';
      });
    });
  }

  ngOnInit() {
    const config = new ConfigurationModel();
    config.enableForgotPassword = true;
    this.configurationService.init(config);
    this.profileService.currentProfile.subscribe(p => {
      this.profile = p;
      this.changeDetectorRef.detectChanges();
    });
  }

  logout() {
    this.auth.logout();
  }

  goToProfile() {
    this.router.navigate(['account', 'profile']);
  }
}
