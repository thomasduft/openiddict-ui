import { Component, isDevMode } from '@angular/core';

import { UserService } from '../../shared';

@Component({
  selector: 'tw-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.less']
})
export class HomeComponent {
  public get isAuthenticated(): boolean {
    return this.user.isAuthenticated;
  }

  public get isAdmin(): boolean {
    return this.user.hasClaim('Administrator');
  }

  public get userName(): string {
    return this.user.userName;
  }

  public constructor(
    private user: UserService
  ) { }

  public openProfile(): void {
    window.location.replace(
      isDevMode()
        ? 'https://localhost:5000/account/manage'
        : window.location.origin + '/account/manage'
    );
  }
}
