import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AppInsightsInterceptor, AuthGuard, ForbiddenPageComponent, NotFoundPageComponent } from 'common';

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
        canActivate: [AppInsightsInterceptor],
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
    loadChildren: 'common/account/account.module#AccountModule'
  },
  {
    path: 'forbidden',
    canActivate: [AppInsightsInterceptor],
    component: ForbiddenPageComponent
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
