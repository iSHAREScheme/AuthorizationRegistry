import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { RoleGuard, constants, AppInsightsInterceptor } from 'common';
import { PoliciesOverviewComponent } from './components/policies-overview/policies-overview.component';
import { ViewPolicyComponent } from './components/view-policy/view-policy.component';
import { CopyPolicyComponent } from './components/copy-policy/copy-policy.component';
import { EditPolicyComponent } from './components/edit-policy/edit-policy.component';
import { CreatePolicyComponent } from './components/create-policy/create-policy.component';

const routes: Routes = [
  {
    path: '',
    component: PoliciesOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      allowedRoles: [constants.roles.EntitledPartyCreator, constants.roles.EntitledPartyViewer, constants.roles.SchemeOwner]
    }
  },
  {
    path: 'view/:id',
    component: ViewPolicyComponent,
    canActivate: [RoleGuard, AppInsightsInterceptor],
    data: {
      allowedRoles: [constants.roles.EntitledPartyCreator, constants.roles.EntitledPartyViewer, constants.roles.SchemeOwner]
    }
  },
  {
    path: 'copy/:id',
    component: CopyPolicyComponent,
    canActivate: [RoleGuard, AppInsightsInterceptor],
    data: { allowedRoles: [constants.roles.EntitledPartyCreator] }
  },
  {
    path: 'edit/:id',
    component: EditPolicyComponent,
    canActivate: [RoleGuard, AppInsightsInterceptor],
    data: { allowedRoles: [constants.roles.EntitledPartyCreator] }
  },
  {
    path: 'create',
    component: CreatePolicyComponent,
    canActivate: [RoleGuard, AppInsightsInterceptor],
    data: { allowedRoles: [constants.roles.EntitledPartyCreator] }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PoliciesRoutingModule {}
