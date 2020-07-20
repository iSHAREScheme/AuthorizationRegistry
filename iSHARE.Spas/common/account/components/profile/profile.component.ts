import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ProfileService } from '@common/generic';
import * as _ from 'lodash';
import { constants } from '@common/constants';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  email: string;
  partyId: string;
  partyName: string;
  schemeOwnerRoles = [];
  authorizationRegistryRoles = [];
  bananaCoRoles = [];
  cttRoles = [];
  constructor(private router: Router, private profile: ProfileService) {
    const profileData = this.profile.get();
    this.email = profileData.email;
    this.partyId = profileData.partyId;
    this.partyName = profileData.partyName;
    const roles = typeof profileData.role === 'string' ? [profileData.role] : profileData.role;
    if (_.find(roles, role => role === constants.roles.SchemeOwner)) {
      this.schemeOwnerRoles = [constants.roles.SchemeOwner];
    }
    this.authorizationRegistryRoles = this.getRolesForCategory(
      constants.categories.AuthorizationRegistry,
      roles
    );
    this.bananaCoRoles = this.getRolesForCategory(constants.categories.BananaCo, roles);
    this.cttRoles = this.getRolesForCategory(constants.categories.ConformaneTestTool, roles);
  }

  ngOnInit() {}

  changePassword() {
    this.router.navigate(['account', 'change-password']);
  }
  getRolesForCategory(category: string, roles: string[]) {
    const roleCategory = _.find(constants.roleCategories, c => c.identifier === category);
    return _.map(
      _.filter(roleCategory.roles, role => !!_.find(roles, r => r === role.value)),
      m => m.displayName
    );
  }
}
