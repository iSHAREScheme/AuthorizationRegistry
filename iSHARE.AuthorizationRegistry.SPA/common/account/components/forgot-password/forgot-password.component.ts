import { Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { AccountService } from '../../services/account.service';
import { ForgotPasswordModel } from '../../models/ForgotPasswordModel';
import { AuthService } from '@generic/index';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss']
})
export class ForgotPasswordComponent implements OnInit {
  emailAddress: string;
  serverError: string;
  success = false;

  ngOnInit(): void {}

  constructor(private api: AccountService, private router: Router, private auth: AuthService) {
    this.emailAddress = '';
  }

  login(): void {
    this.auth.goToLogin();
  }

  sendForgotPasswordEmail(): void {
    const user: ForgotPasswordModel = {
      email: this.emailAddress
    };

    this.api.sendForgotPasswordEmail(user).subscribe(
      response => {
        this.success = true;
      },
      error => {
        if (error.data && error.data.length > 0) {
          this.serverError = error.data;
        } else {
          this.serverError = error.message;
        }
      }
    );
  }
}
