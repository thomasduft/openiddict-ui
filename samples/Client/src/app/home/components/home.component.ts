import { ResourceInfo } from './../models/resource-info.model';
import { HttpWrapperService } from './../../shared/services/httpWrapper.service';
import { Component, isDevMode } from '@angular/core';

import { UserService } from '../../shared';

@Component({
  selector: 'tw-home',
  templateUrl: './home.component.html'
})
export class HomeComponent {
  public static API_SERVICE = 'https://localhost:5001';

  public privateInfo: ResourceInfo;
  public publicInfo: ResourceInfo;

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
    private user: UserService,
    private http: HttpWrapperService
  ) { }

  public openProfile(): void {
    window.location.replace(
      isDevMode()
        ? 'https://localhost:5000/manage'
        : window.location.origin + '/manage'
    );
  }

  public loadData() {
    this.loadPrivateData();
    this.loadPublicData();
  }

  private loadPrivateData(): void {
    const endpoint = `${HomeComponent.API_SERVICE}/api/resource/private`;
    this.http.getRaw<ResourceInfo>(endpoint)
      .subscribe((result: ResourceInfo) => {
        this.privateInfo = result;
      });
  }

  private loadPublicData(): void {
    const endpoint = `${HomeComponent.API_SERVICE}/api/resource/public`;
    this.http.getRaw<ResourceInfo>(endpoint)
      .subscribe((result: ResourceInfo) => {
        this.publicInfo = result;
      });
  }
}
