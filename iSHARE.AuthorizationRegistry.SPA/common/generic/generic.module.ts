import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule as AngularCommonModule } from '@angular/common';
import { NgModule, ModuleWithProviders, APP_INITIALIZER } from '@angular/core';
import { RouterModule } from '@angular/router';

import { RuntimeConfigurationService } from './services/runtime-configuration.service';
import { AppInsightsInterceptor } from './interceptors/AppInsights.interceptor';
import { AppInsightsService } from './services/app-insights.service';

import { AuthService } from './services/auth.service';
import { AuthGuard } from './guards/auth.guard';
import { JwtHttpInterceptorProvider } from './interceptors/JwtHttp.interceptor';
import { ErrorInterceptorProvider } from './interceptors/HttpError.interceptor';
import { LoaderInterceptorProvider } from './interceptors/Loader.interceptor';
import { ProfileService } from './services/profile.service';
import { AuthorizedDirective } from './directives/authorized.directive';
import { RoleGuard } from './guards/role.guard';
import { ForbiddenPageComponent } from './components/forbidden-page/forbidden-page.component';
import { DownloadService } from './services/download.service';
import { JSONService } from './services/JSON.service';

import { HumanizePipe } from './pipes/humanize.pipe';
import { AlertService } from './services/alert.service';
import { NotFoundPageComponent } from './components/not-found-page/not-found-page.component';
import { LeftMenuComponent } from './components/left-menu/left-menu.component';
import { DropdownComponent } from './components/dropdown/dropdown.component';
import { LoaderComponent } from './components/loader/loader.component';
import { SortingComponent } from './components/sorting/sorting.component';
import { SearchComponent } from './components/search/search.component';

import { DisplayErrorsComponent } from './components/display-errors.component';
import { ValidationMessageComponent } from './components/validation-message.component';
import { MinLowercaseValidatorDirective } from './directives/min-lowercase.directive';
import { MinNumericValidatorDirective } from './directives/min-numeric.directive';
import { MinSpecialCharsValidatorDirective } from './directives/min-special-characters.directive';
import { MinUppercaseValidatorDirective } from './directives/min-uppercase.directive';

@NgModule({
  imports: [RouterModule, AngularCommonModule, FormsModule, ReactiveFormsModule],
  declarations: [
    AuthorizedDirective,
    ForbiddenPageComponent,
    LeftMenuComponent,
    LoaderComponent,
    SortingComponent,
    SearchComponent,
    HumanizePipe,
    NotFoundPageComponent,
    DropdownComponent,
    DisplayErrorsComponent,
    ValidationMessageComponent,
    MinLowercaseValidatorDirective,
    MinNumericValidatorDirective,
    MinSpecialCharsValidatorDirective,
    MinUppercaseValidatorDirective
  ],
  exports: [
    AuthorizedDirective,
    ForbiddenPageComponent,
    LeftMenuComponent,
    LoaderComponent,
    SortingComponent,
    SearchComponent,
    HumanizePipe,
    NotFoundPageComponent,
    DropdownComponent,
    DisplayErrorsComponent,
    ValidationMessageComponent,
    MinLowercaseValidatorDirective,
    MinNumericValidatorDirective,
    MinSpecialCharsValidatorDirective,
    MinUppercaseValidatorDirective
  ]
})
export class GenericModule {
  static forRoot(): ModuleWithProviders {
    return {
      ngModule: GenericModule,
      providers: [
        AuthService,
        ProfileService,
        DownloadService,
        JSONService,
        AuthGuard,
        RoleGuard,
        RuntimeConfigurationService,
        AppInsightsInterceptor,
        JwtHttpInterceptorProvider,
        ErrorInterceptorProvider,
        LoaderInterceptorProvider,
        AlertService,
        AppInsightsService
      ]
    };
  }
}
