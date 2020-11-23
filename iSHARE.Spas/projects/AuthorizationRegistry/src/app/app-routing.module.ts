import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {
  ForbiddenPageComponent,
  NotFoundPageComponent,
  AuthCallbackComponent,
  AccessDeniedComponent
} from 'common';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'policies',
    pathMatch: 'full'
  },
  {
    path: '',
    runGuardsAndResolvers: 'always',
    children: [
      {
        path: 'users',
        loadChildren: './users/users.module#UsersModule'
      },
      {
        path: 'policies',
        loadChildren: './policies/policies.module#PoliciesModule'
      },
      {
        path: 'test',
        loadChildren: './mask/mask.module#MaskModule'
      }
    ]
  },
  {
    path: 'account',
    loadChildren: 'common/account/account.module#AccountModule'
  },
  {
    path: 'callback',
    component: AuthCallbackComponent
  },
  {
    path: 'access-denied',
    component: AccessDeniedComponent
  },
  {
    path: 'forbidden',
    component: ForbiddenPageComponent
  },
  {
    path: 'account',
    runGuardsAndResolvers: 'always',
    loadChildren: './login-dispatch/login-dispatch.module#LoginDispatchModule'
  },
  {
    path: '**',
    component: NotFoundPageComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { onSameUrlNavigation: 'reload' })],
  exports: [RouterModule]
})
export class AppRoutingModule {}
