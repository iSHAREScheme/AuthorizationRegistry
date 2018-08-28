export const environment = {
  production: true,
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
