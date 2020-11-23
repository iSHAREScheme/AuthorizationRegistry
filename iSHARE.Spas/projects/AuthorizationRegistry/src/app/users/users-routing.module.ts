import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { UsersOverviewComponent } from './components/users-overview/users-overview.component';
import { CreateUserComponent } from './components/create-user/create-user.component';
import { EditUserComponent } from './components/edit-user/edit-user.component';
import { constants } from '@common/constants';

const routes: Routes = [
  {
    path: '',
    component: UsersOverviewComponent,
    data: {
      allowedRoles: [
        constants.roles.SchemeOwner,
        constants.roles.ArPartyAdmin
      ]
    }
  },
  {
    path: 'create',
    component: CreateUserComponent,
    data: {
      allowedRoles: [
        constants.roles.SchemeOwner,
        constants.roles.ArPartyAdmin
      ]
    }
  },
  {
    path: 'edit/:id',
    component: EditUserComponent,
    data: {
      allowedRoles: [
        constants.roles.SchemeOwner,
        constants.roles.ArPartyAdmin
      ]
    }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UsersRoutingModule {}
