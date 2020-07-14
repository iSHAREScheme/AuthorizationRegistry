import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

import { AppInsights } from 'applicationinsights-js';
import { EnvironmentModel } from '../models/EnvironmentModel';

@Injectable()
export class AppInsightsService {
  constructor(private environment: EnvironmentModel, private router: Router) {
    const config: Microsoft.ApplicationInsights.IConfig = {
      instrumentationKey: this.environment.appInsights.instrumentationKey
    };

    if (!AppInsights.config) {
      AppInsights.downloadAndSetup(config);
    }
  }

  logPageView(name?: string, url?: string, properties?: any, measurements?: any, duration?: number) {
    AppInsights.trackPageView(name, url, properties, measurements, duration);
  }

  logEvent(name: string, properties?: any, measurements?: any) {
    AppInsights.trackEvent(name, properties, measurements);
  }
}
