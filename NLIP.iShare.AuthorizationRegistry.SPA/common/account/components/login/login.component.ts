import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormControl, Validators } from '@angular/forms';

import { AuthService, RuntimeConfigurationService, ConfigurationModel } from '@generic/index';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  serverError: string;
  form: FormGroup;
  config: ConfigurationModel;

  constructor(
    private auth: AuthService,
    private router: Router,
    private route: ActivatedRoute,
    private configService: RuntimeConfigurationService
  ) {
    this.form = new FormGroup({
      username: new FormControl('', [Validators.required]),
      password: new FormControl('', [Validators.required])
    });
  }

  ngOnInit() {
    this.configService.currentConfig.subscribe(config => (this.config = config));

    if (this.auth.isLoggedIn()) {
      this.router.navigateByUrl('');
    } else {
      this.auth.logout();
    }
  }

  login() {
    if (this.form.valid) {
      this.serverError = undefined;
      this.auth.login(this.form.get('username').value, this.form.get('password').value).then(
        () => {
          let returnUrl = this.route.snapshot.queryParams['returnUrl'];
          if (!returnUrl) {
            returnUrl = '';
          }
          this.router.navigateByUrl(returnUrl);
        },
        err => {
          this.serverError = 'Wrong username or password.';
        }
      );
    }
  }

  forgotPassword() {
    this.router.navigateByUrl('account/forgot-password');
  }
}
