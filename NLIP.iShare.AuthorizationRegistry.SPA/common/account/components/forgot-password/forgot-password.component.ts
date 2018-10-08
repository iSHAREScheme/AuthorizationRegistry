import { Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { AccountService } from '../../services/account.service';
import { ForgotPasswordModel } from '../../models/ForgotPasswordModel';
import { AlertService } from '@generic/index';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss']
})
export class ForgotPasswordComponent implements OnInit {
  username: string;
  emailAddress: string;
  serverError: string;

  ngOnInit(): void {}

  constructor(private api: AccountService, private router: Router, private alert: AlertService) {
    this.username = '';
  }

  back(): void {
    this.router.navigate(['']);
  }

  sendForgotPasswordEmail(): void {
    const user: ForgotPasswordModel = {
      username: this.username
    };

    this.api.sendForgotPasswordEmail(user).subscribe(
      response => {
        this.alert.success('Email sent successfully.');
        this.serverError = '';
        this.router.navigateByUrl('account/login');
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
