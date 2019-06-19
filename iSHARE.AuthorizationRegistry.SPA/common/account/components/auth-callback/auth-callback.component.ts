import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AuthService, EnvironmentModel } from '@common/generic';
import { HttpParams, HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-auth-callback',
  templateUrl: './auth-callback.component.html',
  styleUrls: ['./auth-callback.component.scss']
})
export class AuthCallbackComponent implements OnInit {
  environment: any;
  constructor(
    private route: ActivatedRoute,
    private auth: AuthService,
    private config: EnvironmentModel,
    private http: HttpClient
  ) {}

  ngOnInit() {
    this.route.fragment.subscribe(fragment => {
      const code = this.getUrlParameter('code', fragment);
      const identityToken = this.getUrlParameter('id_token', fragment);
      const url = this.config.apiEndpoint + '/account/code';
      this.http.post<any>(url, { code: code }).subscribe(
        response => {
          this.auth.setAccessToken(response['access_token'], identityToken, response['expires_in']);
        },
        error => {
          this.auth.logout();
        }
      );
    });
  }
  getUrlParameter(name, location) {
    const regex = new RegExp(name + '=([^&#]*)');
    const results = regex.exec(location);
    return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
  }
}
