import { LandingPage, AuthService, EnvironmentModel } from '@common/generic';
import { Injectable } from '@angular/core';

@Injectable()
export class DefaultLandingPage implements LandingPage {
  constructor(private auth: AuthService, private environemt: EnvironmentModel) {}
  navigate(): any {
    if (this.environemt.userManagement) {
      this.auth.goToLogin();
    } else {
      this.auth.goToLoginPage();
    }
  }
}
