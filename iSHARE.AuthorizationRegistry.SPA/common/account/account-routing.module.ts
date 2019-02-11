import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';
import { AuthGuard } from '@generic/guards/auth.guard';
import { AppInsightsInterceptor } from '@generic/interceptors/AppInsights.interceptor';

import { ForgotPasswordComponent } from './components/forgot-password/forgot-password.component';
import { ChangePasswordComponent } from './components/change-password/change-password.component';
import { ProfileComponent } from './components/profile/profile.component';
import { ActivateAccountComponent } from './components/activate-account/activate-account.component';
import { LoginComponent } from './components/login/login.component';
import { ResetPasswordComponent } from '@common/account/components/reset-password/reset-password.component';

const routes: Routes = [
  {
    path: 'change-password',
    component: ChangePasswordComponent,
    canActivate: [AuthGuard, AppInsightsInterceptor],
    runGuardsAndResolvers: 'always'
  },
  {
    path: 'profile',
    component: ProfileComponent,
    canActivate: [AuthGuard, AppInsightsInterceptor],
    runGuardsAndResolvers: 'always'
  },
  {
    path: 'activate',
    component: ActivateAccountComponent,
    canActivate: [AppInsightsInterceptor],
    runGuardsAndResolvers: 'always'
  },
  {
    path: 'forgot-password',
    component: ForgotPasswordComponent,
    canActivate: [AppInsightsInterceptor]
  },
  {
    path: 'login',
    component: LoginComponent,
    canActivate: [AppInsightsInterceptor],
    runGuardsAndResolvers: 'always'
  },
  {
    path: 'reset-password',
    component: ResetPasswordComponent,
    canActivate: [AppInsightsInterceptor],
    runGuardsAndResolvers: 'always'
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AccountRoutingModule {}
