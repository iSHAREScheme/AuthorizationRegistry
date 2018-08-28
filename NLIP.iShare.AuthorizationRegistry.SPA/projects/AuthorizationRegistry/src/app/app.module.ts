import { JwtModule } from '@auth0/angular-jwt';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { GenericModule, AccountModule } from 'common';
import { environment } from '@env-ar/environment';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

export function getAccessToken(): string {
  return localStorage.getItem(environment.localStorageKeys.auth);
}

export const jwtConfig = {
  tokenGetter: getAccessToken,
  whitelistedDomains: ['localhost:5000', environment.domain, 'https://reqres.in']
};

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    CommonModule,
    FormsModule,
    AccountModule,
    GenericModule.forRoot(environment),
    // tslint:disable-next-line:max-line-length
    JwtModule.forRoot({ config: jwtConfig }) // this does not work properly for feature modules currently. Used JwtHttpInterceptor for now. look more into it
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule {}
