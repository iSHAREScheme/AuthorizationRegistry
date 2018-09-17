import * as _ from 'lodash';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UsersApiService } from '@app-ar/users/services/users-api.service';
import { User } from '@app-ar/users/models/User';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { constants, AlertService } from 'common';

@Component({
  selector: 'app-edit-user',
  templateUrl: './edit-user.component.html',
  styleUrls: ['./edit-user.component.scss']
})
export class EditUserComponent implements OnInit, OnDestroy {
  private paramsSubscription: any;
  serverError: string;
  form: FormGroup;
  unassignedRoles: string[];
  roles: string[];
  model: { id: string; username: string; email: string; active: string };
  isPartyHidden = false;
  passwordResetEnabled = true;

  constructor(
    private api: UsersApiService,
    private router: Router,
    private route: ActivatedRoute,
    private alert: AlertService
  ) {
    this.initModel();
    this.buildForm();
    this.initRoles();
  }

  ngOnInit() {
    this.paramsSubscription = this.route.params.subscribe(params => {
      this.load(params['id']);
    });
  }

  private load(id: string): void {
    this.api.get(id).subscribe(response => this.bindUser(response));
  }

  save() {
    if (this.form.valid) {
      const user: Partial<User> = {
        id: this.model.id,
        roles: this.roles
      };

      if (!this.isPartyHidden) {
        user.partyId = this.form.get('partyId').value;
        user.partyName = this.form.get('partyName').value;
      }

      this.api.update(user).subscribe(
        () => {
          this.alert.success('Update performed successfully.');
          this.back();
        },
        err => (this.serverError = err.message)
      );
    }
  }

  addRole(role: string): void {
    _.remove(this.unassignedRoles, i => i === role);
    this.roles.push(role);
    this.form.controls['roles'].patchValue(this.roles.join(','));
    this.form.controls['roles'].markAsTouched();
    this.form.controls['roles'].markAsDirty();
  }

  removeRole(role: string): void {
    _.remove(this.roles, i => i === role);
    this.unassignedRoles.push(role);
    this.form.controls['roles'].patchValue(this.roles.join(','));
    this.form.controls['roles'].markAsTouched();
    this.form.controls['roles'].markAsDirty();
  }

  emailChanging() {
    const username = this.form.get('email').value.split('@')[0];
    this.form.controls['username'].patchValue(username);
  }
  sendActivationEmail() {
    const user: Partial<User> = {
      id: this.model.id
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
    this.api.resetPassword(this.model).subscribe(
      () => {
        this.alert.success('user password has been reset');
        this.load(this.model.id);
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

  private buildForm() {
    this.form = new FormGroup({
      partyId: new FormControl('', [Validators.required]),
      partyName: new FormControl('', [Validators.required]),
      roles: new FormControl('', [Validators.required])
    });

    this.form.get('roles').valueChanges.subscribe(value => {
      const roles = value.split(',');
      this.isPartyHidden = roles.length === 1 && roles[0] === constants.roles.SchemeOwner;
      if (this.isPartyHidden) {
        this.form.get('partyId').clearValidators();
        this.form.get('partyName').clearValidators();
      } else {
        this.form.get('partyId').setValidators([Validators.required]);
        this.form.get('partyName').setValidators([Validators.required]);
      }
      this.form.get('partyId').updateValueAndValidity();
      this.form.get('partyName').updateValueAndValidity();
    });
  }

  private initModel() {
    this.model = { id: '', username: '', email: '', active: '' };
  }

  private bindUser(user: User) {
    this.model.id = user.id;
    this.model.username = user.username;
    this.model.email = user.email;
    this.model.active = user.active;
    this.form.controls['partyId'].patchValue(user.partyId);
    this.form.controls['partyName'].patchValue(user.partyName);
    this.form.controls['roles'].patchValue(user.roles.join(','));
    _.remove(this.unassignedRoles, ur => user.roles.some(r => r === ur));
    this.roles = user.roles;
  }

  private initRoles() {
    this.unassignedRoles = [];
    this.roles = [];
    _.forOwn(constants.roles, value => this.unassignedRoles.push(value));
  }
}
