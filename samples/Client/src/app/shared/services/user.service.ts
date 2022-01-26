import { Injectable } from '@angular/core';

import { UserInfo, OAuthService } from 'angular-oauth2-oidc';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private static ANONYMOUS = 'anonymous';

  private username: string = UserService.ANONYMOUS;
  private claims: Array<string> = new Array<string>();

  public get isAuthenticated(): boolean {
    return this.username !== UserService.ANONYMOUS;
  }

  public get userName(): string {
    return this.username;
  }

  public get userClaims(): Array<string> {
    return this.claims;
  }

  public constructor(
    private oauth: OAuthService
  ) { }

  public reset(): void {
    this.loadProfile();
  }

  public hasClaim(claim: string): boolean {
    if (!this.claims || !claim) {
      return false;
    }

    return this.claims.some((r => r === claim));
  }

  public loadProfile(): void {
    if (!this.isAuthenticated) {
      this.oauth.loadUserProfile()
        .then((user: UserInfo) => {
          this.username = user.info.name || user.info.email;
          this.claims = Array.isArray(user.info.role)
            ? user.info.role
            : [user.info.role];

          // if (jwt.tw && Array.isArray(jwt.tw)) {
          //   this.claims.push(...jwt.tw);
          // }
        });
    }
  }
}
