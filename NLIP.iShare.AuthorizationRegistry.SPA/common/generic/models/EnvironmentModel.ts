export class EnvironmentModel {
  production: boolean;
  domain: string;
  appInsights: {
    instrumentationKey: string;
  };
  api: string;
  scope: string;
  localStorageKeys: {
    auth: string;
    profile: string;
    logging: string;
    authExpiration: string;
  };
}
