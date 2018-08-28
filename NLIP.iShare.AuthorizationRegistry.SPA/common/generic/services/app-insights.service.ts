import { Injectable, Inject } from '@angular/core';
import { Router } from '@angular/router';

import { EnvironmentModel } from '../models/EnvironmentModel';
import { AppInsights } from 'applicationinsights-js';

@Injectable()
export class AppInsightsService {
  environment: EnvironmentModel;

  constructor(
    @Inject('environmentProvider') private environmentProvider: EnvironmentModel,
    private router: Router
  ) {
    this.environment = environmentProvider;
    const config: Microsoft.ApplicationInsights.IConfig = {
      instrumentationKey: this.environment.appInsights.instrumentationKey
    };

    if (!AppInsights.config) {
      AppInsights.downloadAndSetup(config);
    }
  }

  logPageView(
    name?: string,
    url?: string,
    properties?: any,
    measurements?: any,
    duration?: number
  ) {
    AppInsights.trackPageView(name, url, properties, measurements, duration);
  }

  logEvent(name: string, properties?: any, measurements?: any) {
    AppInsights.trackEvent(name, properties, measurements);
  }
}
