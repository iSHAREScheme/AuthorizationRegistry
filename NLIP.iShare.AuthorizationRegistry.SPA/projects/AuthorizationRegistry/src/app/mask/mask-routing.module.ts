import { constants, RoleGuard } from 'common';
import { DelegationMaskComponent } from '@app-ar/mask/components/delegation-mask/delegation-mask.component';
import { Routes, RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';

const routes: Routes = [
  {
    path: '',
    component: DelegationMaskComponent,
    canActivate: [RoleGuard],
    data: {
      allowedRoles: [
        constants.roles.EntitledPartyCreator,
        constants.roles.EntitledPartyViewer,
        constants.roles.SchemeOwner
      ]
    }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MaskRoutingModule {}
