import { Subscription } from 'rxjs';

import { Component, OnInit, OnDestroy } from '@angular/core';

import { AutoUnsubscribe, IMessageSubscriber, MessageBus } from '../../../shared';
import { RefreshMessage } from '../../../core';

import { User } from '../../models';
import { AccountService } from '../../services/account.service';

@AutoUnsubscribe
@Component({
  selector: 'tw-user-list',
  templateUrl: './user-list.component.html',
  providers: [
    AccountService
  ]
})
export class UserListComponent
  implements OnInit, OnDestroy, IMessageSubscriber<RefreshMessage> {
  private users$: Subscription;
  private busSubscription: number;

  public searchText = '';
  public users: Array<User> = [];

  public constructor(
    private service: AccountService,
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
    if (message.source === 'user') {
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
    this.loadUsers();
  }

  private loadUsers(): void {
    this.users$ = this.service.users()
      .subscribe((response: Array<User>) => {
        this.users = response;
      });
  }
}
