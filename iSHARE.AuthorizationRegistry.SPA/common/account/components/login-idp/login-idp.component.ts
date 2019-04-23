import { Component, OnInit } from '@angular/core';
import { AuthService } from '@generic/index';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login-idp',
  templateUrl: './login-idp.component.html',
  styleUrls: ['./login-idp.component.scss']
})
export class LoginIdpComponent implements OnInit {
  loaded = false;
  constructor(private auth: AuthService, private router: Router) {}

  ngOnInit() {
    if (this.auth.isLoggedIn()) {
      this.router.navigateByUrl('');
    } else {
      this.loaded = true;
    }
  }
  login() {
    this.auth.goToLogin();
  }
}
