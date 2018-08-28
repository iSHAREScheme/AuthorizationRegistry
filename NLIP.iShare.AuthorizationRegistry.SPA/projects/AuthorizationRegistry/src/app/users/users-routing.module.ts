import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AppInsightsInterceptor, RoleGuard, constants } from 'common';
import { UsersOverviewComponent } from './components/users-overview/users-overview.component';
import { CreateUserComponent } from './components/create-user/create-user.component';
import { EditUserComponent } from './components/edit-user/edit-user.component';

const routes: Routes = [
  {
    path: '',
    component: UsersOverviewComponent,
    canActivate: [RoleGuard],
    data: { allowedRoles: [constants.roles.SchemeOwner] }
  },
  {
    path: 'create',
    component: CreateUserComponent,
    canActivate: [RoleGuard, AppInsightsInterceptor],
    data: { allowedRoles: [constants.roles.SchemeOwner] }
  },
  {
    path: 'edit/:id',
    component: EditUserComponent,
    canActivate: [RoleGuard, AppInsightsInterceptor],
    data: { allowedRoles: [constants.roles.SchemeOwner] }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UsersRoutingModule {}
