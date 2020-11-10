import { Subscription } from 'rxjs';

import { Component, OnInit, OnDestroy } from '@angular/core';

import { AutoUnsubscribe, IMessageSubscriber, MessageBus } from '../../../shared';
import { RefreshMessage } from '../../../core';

import { Client } from '../../models';
import { ClientService } from '../../services/index';

@AutoUnsubscribe
@Component({
  selector: 'tw-client-list',
  templateUrl: './client-list.component.html',
  styleUrls: ['./client-list.component.less'],
  providers: [
    ClientService
  ]
})
export class ClientListComponent
  implements OnInit, OnDestroy, IMessageSubscriber<RefreshMessage> {
  private clients$: Subscription;
  private busSubscription: number;

  public searchText = '';
  public clients: Array<Client> = [];

  public constructor(
    private service: ClientService,
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
    if (message.source === 'application') {
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
    this.clients$ = this.service.clients()
      .subscribe((response: Array<Client>) => {
        this.clients = response;
      });
  }
}
