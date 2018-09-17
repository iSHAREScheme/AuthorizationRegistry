// This file can be replaced during build by using the `fileReplacements` array.
// `ng build ---prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  domain: 'localhost:61433',
  appInsights: {
    instrumentationKey: '00000000-0000-0000-0000-000000000000'
  },
  client_secret: 'secret',
  api: 'http://localhost:61433/',
  scope: 'ar.api',
  localStorageKeys: {
    auth: 'NLIP.iSHARE.auth',
    profile: 'NLIP.iSHARE.profile',
    logging: 'NLIP.iSHARE.logging',
    authExpiration: 'NLIP.iSHARE.authExpiration'
  }
};

/*
 * In development mode, to ignore zone related error stack frames such as
 * `zone.run`, `zoneDelegate.invokeTask` for easier debugging, you can
 * import the following file, but please comment it out in production mode
 * because it will have performance impact when throw error
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
