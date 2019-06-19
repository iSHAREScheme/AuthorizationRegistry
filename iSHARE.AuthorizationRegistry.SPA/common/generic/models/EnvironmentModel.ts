import { Injectable } from '@angular/core';
import { EnvironmentType } from './EnvironmentType';

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
  environmentType: EnvironmentType;
  disablePartyUsersManagement?: boolean;
  identityProvider: {
    authorityUrl: string;
  };
  userManagement: boolean;
}
