import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { constants } from 'common';
import { PoliciesOverviewComponent } from './components/policies-overview/policies-overview.component';
import { ViewPolicyComponent } from './components/view-policy/view-policy.component';
import { CopyPolicyComponent } from './components/copy-policy/copy-policy.component';
import { EditPolicyComponent } from './components/edit-policy/edit-policy.component';
import { CreatePolicyComponent } from './components/create-policy/create-policy.component';

const routes: Routes = [
  {
    path: '',
    component: PoliciesOverviewComponent,
    canActivate: [],
    data: {
      allowedRoles: [
        constants.roles.ArEntitledPartyCreator,
        constants.roles.ArEntitledPartyViewer,
        constants.roles.ArPartyAdmin,
        constants.roles.SchemeOwner
      ]
    }
  },
  {
    path: 'view/:id',
    component: ViewPolicyComponent,
    canActivate: [],
    data: {
      allowedRoles: [
        constants.roles.ArEntitledPartyCreator,
        constants.roles.ArEntitledPartyViewer,
        constants.roles.ArPartyAdmin,
        constants.roles.SchemeOwner
      ]
    }
  },
  {
    path: 'copy/:id',
    component: CopyPolicyComponent,
    canActivate: [],
    data: { allowedRoles: [constants.roles.ArEntitledPartyCreator] }
  },
  {
    path: 'edit/:id',
    component: EditPolicyComponent,
    canActivate: [],
    data: { allowedRoles: [constants.roles.ArEntitledPartyCreator] }
  },
  {
    path: 'create',
    component: CreatePolicyComponent,
    canActivate: [],
    data: { allowedRoles: [constants.roles.ArEntitledPartyCreator] }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PoliciesRoutingModule {}
