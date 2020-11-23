import { Routes, RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { constants } from 'common';
import { DelegationMaskComponent } from './components/delegation-mask/delegation-mask.component';

const routes: Routes = [
  {
    path: '',
    component: DelegationMaskComponent,
    data: {
      allowedRoles: [
        constants.roles.ArEntitledPartyCreator,
        constants.roles.ArEntitledPartyViewer,
        constants.roles.ArPartyAdmin,
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
