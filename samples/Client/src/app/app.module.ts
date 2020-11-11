import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { Routes, RouterModule } from '@angular/router';

import { OAuthModule } from 'angular-oauth2-oidc';

import { SharedModule } from './shared/shared.module';
import { ForbiddenComponent, PageNotFoundComponent, httpInterceptorProviders } from './shared';

import { CoreModule } from './core/core.module';

import { ShellModule } from './shell/shell.module';
import { HomeModule } from './home/home.module';
import { ClaimtypesModule } from './claimtypes/claimtypes.module';
import { RolesModule } from './roles/roles.module';
import { UsersModule } from './users/users.module';
import { ScopessModule } from './scopes/scopes.module';
import { ApplicationsModule } from './applications/applications.module';

import { AppComponent } from './app.component';

const ROUTES: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'forbidden', component: ForbiddenComponent },
  { path: '**', component: PageNotFoundComponent }
];

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    RouterModule.forRoot(ROUTES),
    OAuthModule.forRoot(),
    SharedModule,
    CoreModule,
    ShellModule,
    HomeModule,
    ClaimtypesModule,
    RolesModule,
    UsersModule,
    ScopessModule,
    ApplicationsModule
  ],
  providers: [
    httpInterceptorProviders
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
