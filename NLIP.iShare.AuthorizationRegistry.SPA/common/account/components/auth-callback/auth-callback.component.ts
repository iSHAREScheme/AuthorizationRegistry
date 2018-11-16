import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from '@common/generic';

@Component({
  selector: 'app-auth-callback',
  templateUrl: './auth-callback.component.html',
  styleUrls: ['./auth-callback.component.scss']
})
export class AuthCallbackComponent implements OnInit {
  environment: any;
  constructor(private route: ActivatedRoute, private auth: AuthService) {}

  ngOnInit() {
    this.route.fragment.subscribe(fragment => {
      const currentToken = this.getUrlParameter('access_token', fragment);
      const expires_in = this.getUrlParameter('expires_in', fragment);
      this.auth.setAccessToken(currentToken, expires_in);
    });
  }
  getUrlParameter(name, location) {
    const regex = new RegExp(name + '=([^&#]*)');
    const results = regex.exec(location);
    return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
  }
}
