import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormControl, Validators } from '@angular/forms';

import {
  AuthService,
  RuntimeConfigurationService,
  ConfigurationModel,
  AlertService
} from '@generic/index';
import { AccountService } from '@common/account/services/account.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  form: FormGroup;
  twoFactorForm: FormGroup;
  twoFactorSetupForm: FormGroup;

  config: ConfigurationModel;

  QRcode: string;
  sharedKey: string;

  serverError: string;
  twoFactorServerError: string;
  twoFactorSetupServerError: string;

  showLogin = true;
  show2FaSetup = false;
  show2Fa = false;

  returnUrl: string;

  constructor(
    private auth: AuthService,
    private router: Router,
    private route: ActivatedRoute,
    private configService: RuntimeConfigurationService,
    private account: AccountService,
    private alert: AlertService
  ) {
    this.form = new FormGroup({
      username: new FormControl('', [Validators.required]),
      password: new FormControl('', [Validators.required])
    });
    this.twoFactorForm = new FormGroup({
      twoFactorCode: new FormControl('', [Validators.required])
    });

    this.twoFactorSetupForm = new FormGroup({
      twoFactorSetupCode: new FormControl('', [Validators.required])
    });
  }

  ngOnInit() {
    this.configService.currentConfig.subscribe(config => (this.config = config));

    if (this.auth.isLoggedIn()) {
      this.router.navigateByUrl('');
    }
    this.route.queryParams.subscribe(params => {
      this.returnUrl = params['returnUrl'];
      if (!this.returnUrl) {
        this.auth.goToLogin();
      }
    });
  }

  login() {
    if (this.form.valid) {
      this.serverError = undefined;
      this.auth.login(this.form.get('username').value, this.form.get('password').value).subscribe(
        response => {
          window.location.href = this.returnUrl;
        },
        err => {
          if (err.status === 409) {
            this.showLogin = false;
            this.show2Fa = true;
            this.show2FaSetup = false;
          } else if (err.status === 422) {
            this.showLogin = false;
            this.show2Fa = false;
            this.show2FaSetup = true;
            this.getKey();
          } else {
            this.serverError = 'Wrong username or password.';
          }
        }
      );
    }
  }

  login2Fa() {
    if (this.form.valid && this.twoFactorForm.valid) {
      this.serverError = undefined;
      this.auth
        .loginWith2Fa(
          this.form.get('username').value,
          this.form.get('password').value,
          this.twoFactorForm.get('twoFactorCode').value
        )
        .subscribe(
          response => {
            window.location.href = this.returnUrl;
          },
          err => {
            this.twoFactorServerError = 'Invalid code';
          }
        );
    }
  }

  forgotPassword() {
    this.router.navigateByUrl('account/forgot-password');
  }
  back() {
    this.showLogin = true;
    this.show2Fa = false;
    this.show2FaSetup = false;
    this.reset2FaFields();
  }

  enable2fa() {
    this.account
      .enable2fa(
        this.form.get('username').value,
        this.form.get('password').value,
        this.twoFactorSetupForm.get('twoFactorSetupCode').value
      )
      .subscribe(
        response => {
          this.alert.success('Two factor successfully enabled.');
          this.back();
        },
        error => {
          this.twoFactorSetupServerError = 'Invalid code';
        }
      );
  }

  getKey() {
    this.account
      .getAuthenticatorKey(this.form.get('username').value, this.form.get('password').value)
      .subscribe(
        response => {
          this.QRcode = response['authenticatorUri'];
          this.sharedKey = response['sharedKey'];
        },
        error => {
          if (error.status === 400) {
            this.alert.error('Two factor already enabled.');
          }
        }
      );
  }
  reset2FaFields() {
    this.twoFactorForm.reset();
    this.twoFactorSetupForm.reset();
    this.QRcode = '';
    this.sharedKey = '';
    this.twoFactorServerError = '';
    this.twoFactorSetupServerError = '';
  }
}
