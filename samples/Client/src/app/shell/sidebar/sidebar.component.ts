import {
  Component,
  HostBinding,
  Output,
  EventEmitter,
  isDevMode
} from '@angular/core';

import { UserService } from '../../shared';

@Component({
  selector: 'tw-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.less']
})
export class SidebarComponent {
  public collapsed = false;

  public get isAuthenticated(): boolean {
    return this.user.isAuthenticated;
  }

  public get isAdmin(): boolean {
    return this.user.hasClaim('Administrator');
  }

  public get userName(): string {
    return this.user.userName;
  }

  @HostBinding('class')
  public classlist = this.getClassList();

  @Output()
  public loginClick: EventEmitter<string> = new EventEmitter<string>();

  public constructor(
    private user: UserService
  ) { }

  public toggle(): void {
    this.collapsed = !this.collapsed;
    this.classlist = this.getClassList();
  }

  public login(): void {
    this.loginClick.next('login');
  }

  public logout(): void {
    this.loginClick.next('logout');
  }

  private getClassList(): string {
    if (this.collapsed) {
      return 'sidebar sidebar--collapsed';
    }

    return 'sidebar';
  }
}
