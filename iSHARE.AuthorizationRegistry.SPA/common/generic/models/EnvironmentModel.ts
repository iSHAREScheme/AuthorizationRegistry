import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class EnvironmentModel {
  production: boolean;
  apiDomain: string;
  apiEndpoint: string;
  appInsights: {
    instrumentationKey: string;
  };
  scope: string;
  spaUrl: string;
  clientId: string;
}
