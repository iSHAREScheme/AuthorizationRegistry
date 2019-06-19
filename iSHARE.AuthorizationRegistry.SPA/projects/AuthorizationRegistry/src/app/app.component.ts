import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { AuthService, ProfileService, Profile, EnvironmentModel } from '@common/index';
import { AppInsightsService } from 'common';
import { Router } from '@angular/router';
import {
  RuntimeConfigurationService,
  ConfigurationModel
} from '@generic/services/runtime-configuration.service';
import { MENU_ITEMS } from './menu/menu-items';
import { HttpClient } from '@angular/common/http';
import { AuthServiceOptions } from '@common/generic/models/AuthServiceOptions';
import * as _ from 'lodash';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  profile: Profile;
  menuItems = [];
  private options: AuthServiceOptions;

  constructor(
    private auth: AuthService,
    private configurationService: RuntimeConfigurationService,
    private profileService: ProfileService,
    // Tracking is not properly enabled without injecting AppInsightsService
    private appInsightsService: AppInsightsService,
    private router: Router,
    private changeDetectorRef: ChangeDetectorRef,
    private config: EnvironmentModel,
    private http: HttpClient
  ) {
    appInsights.queue.push(() => {
      appInsights.context.addTelemetryInitializer(envelope => {
        envelope.tags['ai.cloud.role'] = 'ar.spa';
      });
    });
    this.options = new AuthServiceOptions(this.config);
    if (this.config.userManagement) {
      this.menuItems = MENU_ITEMS;
    } else {
      this.menuItems = _.filter(MENU_ITEMS, x => x.text !== 'Users');
    }
    this.auth.registerReload(this);
  }

  ngOnInit() {
    const config = new ConfigurationModel();
    config.enableForgotPassword = true;
    this.configurationService.init(config);
    if (!this.auth.isLoggedIn()) {
      this.profileService.clear();
    }
    this.profileService.currentProfile.subscribe(p => {
      this.profile = p;
      this.changeDetectorRef.detectChanges();
    });
  }

  logout() {
    if (this.config.userManagement) {
      this.http
        .post<any>(this.options.logoutEndpoint, null, { withCredentials: true })
        .subscribe(() => {
          this.auth.clearLogout();
          this.auth.goToLogin();
        });
    } else {
      this.auth.logout();
    }
  }

  goToProfile() {
    if (this.config.userManagement) {
      this.router.navigate(['account', 'profile']);
    }
  }
}
