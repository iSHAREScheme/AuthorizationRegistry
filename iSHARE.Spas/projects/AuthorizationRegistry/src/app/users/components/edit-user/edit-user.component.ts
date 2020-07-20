import * as _ from 'lodash';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AlertService, AuthService, EnvironmentModel } from '@common/generic';
import { constants } from '@common/constants';
import { UsersApiService } from '../../services/users-api.service';
import { User } from '../../models/User';
import { Utils } from '@app-ar/users/utils';

@Component({
  selector: 'app-edit-user',
  templateUrl: './edit-user.component.html',
  styleUrls: ['./edit-user.component.scss']
})
export class EditUserComponent implements OnInit, OnDestroy {
  private paramsSubscription: any;
  user: Partial<User> = {};
  serverError: string;
  isPartyHidden = false;
  passwordResetEnabled = true;
  loading = true;
  isSchemeOwner: boolean;
  makeSchemeOwner: boolean;
  selectedRoles: any[] = [];
  roles = [];
  partiesTypeaheadOptions = {
    enableCheckAll: false,
    allowSearchFilter: true,
    multiselect: false,
    singleSelection: true
  };
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
  internalRoles = [];

  constructor(
    private api: UsersApiService,
    private router: Router,
    private route: ActivatedRoute,
    private alert: AlertService,
    private auth: AuthService
  ) {
    this.initRoles();
  }

  ngOnInit() {
    this.paramsSubscription = this.route.params.subscribe(params => {
      this.load(params['id']);
    });
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

  private load(id: string): void {
    this.api.get(id).subscribe(
      response => {
        this.bindUser(response);
        this.loading = false;
      },
      err => {
        this.loading = false;
        if (err.status === 404) {
          this.router.navigate(['not-found'], { skipLocationChange: true });
        }
      }
    );
  }
  private bindUser(response: User) {
    this.user.email = response.email;
    if (_.find(response.roles, x => x === constants.roles.SchemeOwner)) {
      _.remove(response.roles, x => x === constants.roles.SchemeOwner);
      this.makeSchemeOwner = true;
    }
    this.user.id = response.id;
    this.user.active = response.active;
    this.user.roles = [];
    this.user.partyId = response.partyId;
    this.user.partyName = response.partyName;
    const internalRoles = [].concat.apply([], _.map(constants.roleCategories, x => x.roles));
    for (const role of response.roles) {
      const internalRole = _.find(internalRoles, x => x.value === role);
      this.selectedRoles.push(internalRole);
    }
  }

  save() {
    if (this.makeSchemeOwner || this.selectedRoles.length) {
      if (this.makeSchemeOwner) {
        this.user.roles = [constants.roles.SchemeOwner];
      } else {
        this.user.roles = this.user.roles.concat(_.map(this.selectedRoles, r => r.value));
      }

      this.api.update(this.user).subscribe(
        () => {
          this.alert.success('Update performed successfully.');
          this.back();
        },
        err => (this.serverError = err.data)
      );
    }
  }

  sendActivationEmail() {
    const user: Partial<User> = {
      id: this.user.id
    };
    this.api.sendActivate(user).subscribe(
      () => {
        this.alert.success('Email sent successfully.');
      },
      err => {
        this.serverError = err;
      }
    );
  }

  resetUserPassword() {
    this.api.resetPassword(this.user).subscribe(
      () => {
        this.alert.success('User password has been reset.');
        this.load(this.user.id);
      },
      err => {
        this.serverError = err;
      }
    );
  }

  back() {
    this.router.navigate(['users']);
  }

  ngOnDestroy() {
    this.paramsSubscription.unsubscribe();
  }
}
