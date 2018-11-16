import { EnvironmentModel } from '../models/EnvironmentModel';

export class AppConfigService {
  private environments: EnvironmentModel[];
  private defaultEnv: EnvironmentModel;
  constructor(environments: EnvironmentModel[], defaultEnv: EnvironmentModel) {
    this.environments = environments.map(this.trimSlashes);
    this.defaultEnv = this.trimSlashes(defaultEnv);
  }

  private trimSlashes(config: EnvironmentModel) {
    config.apiEndpoint = config.apiEndpoint.replace(/\/+$/, '');
    config.apiDomain = config.apiDomain.replace(/\/+$/, '');
    config.spaUrl = config.spaUrl.replace(/\/+$/, '');
    return config;
  }
  private getHostname(endpoint): string {
    return endpoint.replace(/http[s]*:\/*/, '');
  }

  get(): EnvironmentModel {
    const configKey = window.location.hostname;
    const configEntry = this.environments.find(m => this.getHostname(m.spaUrl) === configKey) || this.defaultEnv;
    return configEntry;
  }
}
