import { GenericModule } from '@generic/index';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

import { AccountRoutingModule } from './account-routing.module';
import { AccountService } from './services/account.service';
import { ProfileComponent } from './components/profile/profile.component';
import { ChangePasswordComponent } from './components/change-password/change-password.component';
import { ActivateAccountComponent } from './components/activate-account/activate-account.component';
import { ForgotPasswordComponent } from './components/forgot-password/forgot-password.component';
import { LoginComponent } from './components/login/login.component';
import { ResetPasswordComponent } from './components/reset-password/reset-password.component';

@NgModule({
  imports: [AccountRoutingModule, FormsModule, CommonModule, ReactiveFormsModule, GenericModule],
  declarations: [
    ProfileComponent,
    ChangePasswordComponent,
    ActivateAccountComponent,
    ForgotPasswordComponent,
    LoginComponent,
    ResetPasswordComponent
  ],
  exports: [
    ProfileComponent,
    ChangePasswordComponent,
    ActivateAccountComponent,
    ForgotPasswordComponent,
    LoginComponent,
    ResetPasswordComponent
  ],
  providers: [AccountService]
})
export class AccountModule {}
