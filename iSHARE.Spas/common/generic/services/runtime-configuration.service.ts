import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';
import { Injectable } from '@angular/core';

@Injectable()
export class RuntimeConfigurationService {
  private config = new BehaviorSubject<ConfigurationModel>(new ConfigurationModel());
  currentConfig = this.config.asObservable();

  contructor() { }

  init(configuration: ConfigurationModel) {
    this.config.next(configuration);
  }
}

export class ConfigurationModel {
  enableForgotPassword: boolean;

  constructor() {
    this.enableForgotPassword = true;
  }
}
