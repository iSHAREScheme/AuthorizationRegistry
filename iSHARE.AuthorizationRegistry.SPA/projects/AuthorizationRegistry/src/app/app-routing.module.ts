import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {
  AppInsightsInterceptor,
  AuthGuard,
  ForbiddenPageComponent,
  NotFoundPageComponent,
  AuthCallbackComponent,
  AccessDeniedComponent,
  LoginIdpComponent,
  LoginComponent
} from 'common';
import { SelfAuthGuard } from '@common/generic/guards/self-auth.guard';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'policies',
    pathMatch: 'full'
  },
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [
      {
        path: 'users',
        canActivate: [AppInsightsInterceptor, SelfAuthGuard],
        loadChildren: './users/users.module#UsersModule'
      },
      {
        path: 'policies',
        canActivate: [AppInsightsInterceptor],
        loadChildren: './policies/policies.module#PoliciesModule'
      },
      {
        path: 'test',
        canActivate: [AppInsightsInterceptor],
        loadChildren: './mask/mask.module#MaskModule'
      }
    ]
  },
  {
    path: 'account',
    canActivate: [SelfAuthGuard],
    loadChildren: 'common/account/account.module#AccountModule'
  },
  {
    path: 'callback',
    canActivate: [AppInsightsInterceptor],
    component: AuthCallbackComponent
  },
  {
    path: 'access-denied',
    canActivate: [AppInsightsInterceptor],
    component: AccessDeniedComponent
  },
  {
    path: 'forbidden',
    canActivate: [AppInsightsInterceptor],
    component: ForbiddenPageComponent
  },
  {
    path: 'account',
    runGuardsAndResolvers: 'always',
    loadChildren: './login-dispatch/login-dispatch.module#LoginDispatchModule'
  },
  {
    path: '**',
    canActivate: [AppInsightsInterceptor],
    component: NotFoundPageComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { onSameUrlNavigation: 'reload' })],
  exports: [RouterModule]
})
export class AppRoutingModule {}
