import { Component, OnInit, HostBinding, isDevMode } from '@angular/core';
import { Router } from '@angular/router';

import { OAuthService, OAuthEvent } from 'angular-oauth2-oidc';

import { UserService } from './shared';

@Component({
  selector: 'tw-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.less']
})
export class AppComponent implements OnInit {
  title = 'ClientApp';

  @HostBinding('class')
  public style = 'shell';

  public get isAuthenticated(): boolean {
    return this.user.isAuthenticated;
  }

  public get username(): string {
    return this.user.userName;
  }

  public get claims(): Array<string> {
    return this.user.userClaims;
  }

  public constructor(
    private router: Router,
    private user: UserService,
    private oauthService: OAuthService
  ) {
  }

  public ngOnInit(): void {
    this.configure();
  }

  public loginClick(state: string): void {
    if (state === 'login') {
      this.login();
    } else {
      this.logout();
    }
  }

  public login(): void {
    this.oauthService.initLoginFlow();
  }

  public logout(): void {
    this.oauthService.logOut(false);
    this.user.reset();
    this.router.navigate(['/']);
  }

  private async configure() {
    const redirUri = isDevMode()
      ? 'http://localhost:4200'
      : window.location.origin;

    const devModeIssuer = 'https://localhost:5000/';

    this.oauthService.configure({
      clientId: 'stsclient',
      issuer: isDevMode()
        ? devModeIssuer
        : window.location.origin,
      redirectUri: redirUri,
      responseType: 'code',
      scope: 'openid sts_api',
      loginUrl: isDevMode()
        ? devModeIssuer + '/identity/account/login'
        : window.location.origin + 'identity/account/login',
      logoutUrl: isDevMode()
        ? devModeIssuer + '/identity/account/logout'
        : window.location.origin + '/identity/account/logout',
      requireHttps: false
    });
    this.oauthService.events.subscribe(async (e: OAuthEvent) => {
      // console.log(e);
      if (e.type === 'token_received' || e.type === 'token_refreshed') {
        this.user.setProperties(this.oauthService.getAccessToken());
      }

      if (e.type === 'discovery_document_loaded' && this.oauthService.hasValidAccessToken()) {
        // const userInfo = await this.oauthService.loadUserProfile();
        // console.log(userInfo);
        this.user.setProperties(this.oauthService.getAccessToken());
      }
    });
    this.oauthService.loadDiscoveryDocumentAndLogin({
      onTokenReceived: context => {
        this.user.setProperties(context.accessToken);
      }
    });
  }
}
