import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import * as _ from 'lodash';
import { constants, AlertService } from 'common';
import { Router } from '@angular/router';
import { UsersApiService } from '@app-ar/users/services/users-api.service';
import { User } from '@app-ar/users/models/User';

@Component({
  selector: 'app-create-user',
  templateUrl: './create-user.component.html',
  styleUrls: ['./create-user.component.scss']
})
export class CreateUserComponent implements OnInit {
  serverError: string;
  form: FormGroup;
  unassignedRoles: string[];
  roles: string[];
  isPartyHidden = false;

  constructor(private api: UsersApiService, private router: Router, private alert: AlertService) {
    this.buildForm();
    this.initRoles();
  }

  save() {
    if (this.form.valid) {
      const user: Partial<User> = {
        email: this.form.get('email').value,
        username: this.form.get('username').value,
        roles: this.roles
      };

      if (!this.isPartyHidden) {
        user.partyId = this.form.get('partyId').value;
        user.partyName = this.form.get('partyName').value;
      }

      this.api.create(user).subscribe(
        () => {
          this.alert.success('Creation performed successfully.');
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
  }

  removeRole(role: string): void {
    _.remove(this.roles, i => i === role);
    this.unassignedRoles.push(role);
    this.form.controls['roles'].patchValue(this.roles.join(','));
  }

  emailChanging() {
    const username = this.form.get('email').value.split('@')[0];
    this.form.controls['username'].patchValue(username);
  }

  back() {
    this.router.navigate(['users']);
  }

  private buildForm() {
    this.form = new FormGroup({
      username: new FormControl('', [Validators.required]),
      email: new FormControl('', [Validators.required, Validators.email]),
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

  private initRoles() {
    this.unassignedRoles = [];
    this.roles = [];
    _.forOwn(constants.roles, value => this.unassignedRoles.push(value));
  }

  ngOnInit() {}
}
