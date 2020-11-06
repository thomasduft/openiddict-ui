import { Injectable } from '@angular/core';

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
      const jwt = JSON.parse(window.atob(accesToken.split('.')[1]));

      this.username = jwt.given_name;

      this.claims = Array.isArray(jwt.role)
        ? jwt.role
        : [jwt.role];

      if (jwt.tw && Array.isArray(jwt.tw)) {
        this.claims.push(...jwt.tw);
      }

      return;
    }

    this.username = UserService.ANONYMOUS;
    this.claims = new Array<string>();
  }
}
