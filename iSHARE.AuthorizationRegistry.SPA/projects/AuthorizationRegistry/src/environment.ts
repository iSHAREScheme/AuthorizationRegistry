/*
 * In development mode, to ignore zone related error stack frames such as
 * `zone.run`, `zoneDelegate.invokeTask` for easier debugging, you can
 * import the following file, but please comment it out in production mode
 * because it will have performance impact when throw error
 */
// import 'zone.js/dist/zone-error'; // Included with Angular CLI.import { EnvironmentModel } from '@common/';
import { EnvironmentModel, AppConfigService, EnvironmentType } from 'common';

// This file can be replaced during build by using the `fileReplacements` array.
// `ng build ---prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environmentConfigurationMap: EnvironmentModel[] = [
  {
    production: false,
    apiDomain: 'localhost:61433',
    appInsights: {
      instrumentationKey: '00000000-0000-0000-0000-000000000000'
    },
    apiEndpoint: 'http://localhost:61433/',
    scope: 'ar.api',
    spaUrl: 'http://localhost:4201',
    clientId: 'AR_SPA',
    disablePartyUsersManagement: false,
    environmentType: EnvironmentType.Development,
    identityProvider: {
      authorizeEndpoint: 'http://localhost:61433/connect/authorize',
    },
    userManagement: true
  }
];
export function environmentFactory() {
  const configService = new AppConfigService(environmentConfigurationMap, environmentConfigurationMap[0]);
  const env = configService.get();
  return env;
}
