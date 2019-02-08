import { EnvironmentModel } from './EnvironmentModel';

export class AuthServiceOptions {
  authorizeEndpoint: string;
  loginEndpoint: string;
  logoutEndpoint: string;
  clientId: string;
  redirectEndpoint: string;
  scope: string;
  constructor(config: EnvironmentModel) {
    this.authorizeEndpoint = `${config.apiEndpoint}/connect/authorize`;
    this.loginEndpoint = `${config.apiEndpoint}/account/login`;
    this.logoutEndpoint = `${config.apiEndpoint}/account/logout`;
    this.clientId = config.clientId;
    this.scope = config.scope;
    this.redirectEndpoint = `${config.spaUrl}/admin/callback`;
  }
}
