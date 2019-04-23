import { Component, OnInit } from '@angular/core';
import * as _ from 'lodash';
import { Router } from '@angular/router';
import { AlertService, EnvironmentModel, AuthService } from '@common/generic';
import { constants } from '@common/constants';
import { UsersApiService } from '../../services/users-api.service';
import { User } from '../../models/User';
import { Utils } from '@app-ar/users/utils';

@Component({
  selector: 'app-create-user',
  templateUrl: './create-user.component.html',
  styleUrls: ['./create-user.component.scss']
})
export class CreateUserComponent implements OnInit {
  serverError: string;
  isPartyHidden = false;
  user: Partial<User> = {};
  isSchemeOwner: boolean;
  makeSchemeOwner: boolean;
  selectedRoles: any[] = [];
  roles = [];
  environment: EnvironmentModel;

  rolesOptions = {
    enableCheckAll: false,
    allowSearchFilter: true,
    idField: 'value',
    textField: 'displayName',
    itemsShowLimit: 1,
    showSelectedItemsAtTop: false,
    noDataAvailablePlaceholderText: 'Loading...'
  };
  constructor(
    private api: UsersApiService,
    private router: Router,
    private alert: AlertService,
    private auth: AuthService
  ) {
    this.initRoles();
    this.user.roles = [];
  }
  initRoles() {
    const items = [];
    if (this.auth.inRole([constants.roles.SchemeOwner])) {
      this.isSchemeOwner = true;
      Utils.addRole(items, constants.categories.AuthorizationRegistry);
    } else {
      if (this.auth.inRole([constants.roles.ArPartyAdmin])) {
        Utils.addRole(items, constants.categories.AuthorizationRegistry);
      }
    }

    this.roles = Utils.createRoleCategories(items);
  }

  save() {
    if (this.user.email) {
      this.user.username = this.user.email;
      if (this.makeSchemeOwner) {
        this.user.roles = [constants.roles.SchemeOwner];
      } else {
        this.user.roles = this.user.roles.concat(_.map(this.selectedRoles, r => r.value));
      }

      this.api.create(this.user).subscribe(
        () => {
          this.alert.success('Creation performed successfully.');
          this.back();
        },
        err => (this.serverError = err.message)
      );
    }
  }

  back() {
    this.router.navigate(['users']);
  }

  ngOnInit() {}
}
