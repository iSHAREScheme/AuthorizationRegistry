export class EnvironmentModel {
  production: boolean;
  domain: string;
  appInsights: {
    instrumentationKey: string;
  };
  client_secret: string;
  api: string;
  scope: string;
  localStorageKeys: {
    auth: string;
    profile: string;
    logging: string;
    authExpiration: string;
  };
}
