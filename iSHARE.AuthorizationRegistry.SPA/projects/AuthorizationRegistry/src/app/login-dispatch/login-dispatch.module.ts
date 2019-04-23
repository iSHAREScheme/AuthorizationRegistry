import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { AppInsightsInterceptor } from '@common/generic';
import { LoginIdpComponent, LoginComponent, AccountModule } from '@common/account';
import { loginMatcher } from './login-matcher';
const routes: Routes = [
  {
    path: 'login',
    canActivate: [AppInsightsInterceptor],
    matcher: loginMatcher,
    component: LoginComponent
  },
  {
    path: 'login',
    canActivate: [AppInsightsInterceptor],
    component: LoginIdpComponent
  }
];
@NgModule({
  imports: [CommonModule, RouterModule.forChild(routes), AccountModule],
  declarations: []
})
export class LoginDispatchModule {}
