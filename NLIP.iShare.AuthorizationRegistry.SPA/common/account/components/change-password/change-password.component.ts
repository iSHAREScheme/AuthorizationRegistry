import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AlertService } from '@generic/index';
import { AccountService } from '../../services/account.service';
import { ChangePasswordModel } from '../../models/ChangePasswordModel';
import { Subscription } from '../../../../node_modules/rxjs';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss']
})
export class ChangePasswordComponent implements OnInit {
  model: ChangePasswordModel;
  serverError: string[];
  navigationSubscription: Subscription;
  constructor(private router: Router, private api: AccountService, private alert: AlertService) {
    this.model = new ChangePasswordModel();
  }

  ngOnInit() {}

  save() {
    this.api.changePassword(this.model).subscribe(
      response => {
        this.alert.success('The password has been successfully changed!');
        this.goToProfile();
      },
      error => {
        if (error.data && error.data.length > 0) {
          this.serverError = error.data;
        } else {
          this.serverError = [error.message];
        }
      }
    );
  }

  goToProfile() {
    this.router.navigate(['account', 'profile']);
  }
}
