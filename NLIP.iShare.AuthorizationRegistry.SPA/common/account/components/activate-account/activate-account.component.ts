import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { Subscription } from 'rxjs';
import { AuthService } from '@generic/index';
import { AccountService } from '../../services/account.service';
import { ActivateAccountModel } from '../../models/ActivateAccountModel';

@Component({
  selector: 'app-activate-account',
  templateUrl: './activate-account.component.html',
  styleUrls: ['./activate-account.component.scss']
})
export class ActivateAccountComponent implements OnInit, OnDestroy {
  model: ActivateAccountModel;
  serverError: string[] = null;
  success = false;
  navigationSubscription: Subscription;

  constructor(private route: ActivatedRoute, private api: AccountService, private router: Router, private auth: AuthService) {
    this.navigationSubscription = this.router.events.subscribe((e: any) => {
      if (e instanceof NavigationEnd) {
        this.initialize();
      }
    });
  }
  initialize() {
    this.auth.clearLogout();
    this.model = new ActivateAccountModel();
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

  activate() {
    this.api.activate(this.model).subscribe(
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
    const url = `account/activate?uid=${this.model.id}&token=${this.model.token}&t=${new Date().toJSON()}`;
    this.router.navigateByUrl(url);
  }
}
