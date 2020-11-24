import { Subscription } from 'rxjs';

import { Component, OnInit, OnDestroy } from '@angular/core';

import { AutoUnsubscribe, IMessageSubscriber, MessageBus } from '../../../shared';
import { RefreshMessage } from '../../../core';

import { Role } from '../../models';
import { RoleService } from '../../services/role.service';

@AutoUnsubscribe
@Component({
  selector: 'tw-role-list',
  templateUrl: './role-list.component.html',
  providers: [
    RoleService
  ]
})
export class RoleListComponent
  implements OnInit, OnDestroy, IMessageSubscriber<RefreshMessage> {
  private roles$: Subscription;
  private busSubscription: number;

  public searchText = '';
  public roles: Array<Role> = [];

  public constructor(
    private service: RoleService,
    private bus: MessageBus
  ) {
    this.busSubscription = this.bus.subsribe(this);
  }

  public ngOnInit(): void {
    this.searchText = '';

    this.loadData();
  }

  public ngOnDestroy(): void {
    this.bus.unsubscribe(this.busSubscription);
  }

  public onMessage(message: RefreshMessage): void {
    if (message.source === 'role') {
      this.loadData();
    }
  }

  public getType(): string {
    return RefreshMessage.KEY;
  }

  public reload(): void {
    this.ngOnInit();
  }

  private loadData(): void {
    this.loadRoles();
  }

  private loadRoles(): void {
    this.roles$ = this.service.roles()
      .subscribe((response: Array<Role>) => {
        this.roles = response;
      });
  }
}
