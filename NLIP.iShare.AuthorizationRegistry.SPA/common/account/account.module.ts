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
import { PasswordRulesComponent } from './components/password-rules/password-rules.component';
import { AuthCallbackComponent } from './components/auth-callback/auth-callback.component';
import { AccessDeniedComponent } from './components/access-denied/access-denied.component';
import { NgxQRCodeModule } from 'ngx-qrcode2';

@NgModule({
  imports: [AccountRoutingModule, FormsModule, CommonModule, ReactiveFormsModule, GenericModule, NgxQRCodeModule],
  declarations: [
    ProfileComponent,
    ChangePasswordComponent,
    ActivateAccountComponent,
    ForgotPasswordComponent,
    LoginComponent,
    ResetPasswordComponent,
    PasswordRulesComponent,
    AuthCallbackComponent,
    AccessDeniedComponent
  ],
  exports: [
    ProfileComponent,
    ChangePasswordComponent,
    ActivateAccountComponent,
    ForgotPasswordComponent,
    LoginComponent,
    ResetPasswordComponent,
    PasswordRulesComponent,
    AuthCallbackComponent,
    AccessDeniedComponent
  ],
  providers: [AccountService]
})
export class AccountModule {}
