import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { Subscription } from 'rxjs';
import { AuthService } from '@generic/index';
import { ResetPasswordModel } from '../../models/ResetPasswordModel';
import { AccountService } from '../../services/account.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss']
})
export class ResetPasswordComponent implements OnInit, OnDestroy {
  model: ResetPasswordModel;
  serverError: string[] = null;
  success = false;
  navigationSubscription: Subscription;

  constructor(
    private route: ActivatedRoute,
    private api: AccountService,
    private router: Router,
    private auth: AuthService
  ) {
    this.navigationSubscription = this.router.events.subscribe((e: any) => {
      if (e instanceof NavigationEnd) {
        this.initialize();
      }
    });
  }
  initialize() {
    this.auth.clearLogout();
    this.model = new ResetPasswordModel();
    this.serverError = null;
    this.success = false;
    this.route.queryParams.subscribe(params => {
      this.success = params['success'] === 'true';
      this.model.token = params['token'];
      this.model.id = params['uid'];
    });
  }

  ngOnInit() {}
  ngOnDestroy() {
    if (this.navigationSubscription) {
      this.navigationSubscription.unsubscribe();
    }
  }

  confirmPasswordReset() {
    this.api.confirmPasswordReset(this.model).subscribe(
      response => {
        this.success = true;
      },
      error => {
        if (error.data && !error.data.success && error.data.errors) {
          this.serverError = error.data.errors;
        } else {
          this.serverError = error.data;
        }
      }
    );
  }
  login() {
    this.router.navigate(['account', 'login']);
  }
  reload() {
    const url = `account/reset-password?uid=${this.model.id}&token=${this.model.token}&t=${new Date().toJSON()}`;
    this.router.navigateByUrl(url);
  }
}
