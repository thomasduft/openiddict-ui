import { Injectable } from '@angular/core';
import { UserInfo } from 'angular-oauth2-oidc';
import { HttpWrapperService } from './httpWrapper.service';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private static ANONYMOUS = 'anonymous';

  private authenticated = false;
  private username: string = UserService.ANONYMOUS;
  private claims: Array<string> = new Array<string>();

  public get isAuthenticated(): boolean {
    return this.authenticated || this.username !== UserService.ANONYMOUS;
  }

  public get userName(): string {
    return this.username;
  }

  public get userClaims(): Array<string> {
    return this.claims;
  }

  public constructor(
    private http: HttpWrapperService
  ) { }

  public reset(): void {
    this.setProperties();
  }

  public hasClaim(claim: string): boolean {
    if (!this.claims || !claim) {
      return false;
    }

    return this.claims.some((r => r === claim));
  }

  public setProperties(accesToken: string = null): void {
    if (accesToken) {
      // const jwt = JSON.parse(window.atob(accesToken.split('.')[1]));

      // this.username = jwt.given_name;

      // this.claims = Array.isArray(jwt.role)
      //   ? jwt.role
      //   : [jwt.role];

      // if (jwt.tw && Array.isArray(jwt.tw)) {
      //   this.claims.push(...jwt.tw);
      // }
      if (!this.isAuthenticated) {
        this.http.getRaw<UserInfo>('https://localhost:5000/connect/userinfo')
          .subscribe((info: UserInfo) => {
            this.username = info.given_name;

          });
      }

      this.authenticated = true;

      return;
    }

    this.username = UserService.ANONYMOUS;
    this.claims = new Array<string>();
  }
}
