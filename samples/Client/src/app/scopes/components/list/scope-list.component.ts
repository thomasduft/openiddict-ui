import { Subscription } from 'rxjs';

import { Component, OnInit, OnDestroy } from '@angular/core';

import { AutoUnsubscribe, IMessageSubscriber, MessageBus } from '../../../shared';
import { RefreshMessage } from '../../../core';

import { Scope } from '../../models';
import { ScopeService } from '../../services/index';

@AutoUnsubscribe
@Component({
  selector: 'tw-scope-list',
  templateUrl: './scope-list.component.html',
  styleUrls: ['./scope-list.component.less'],
  providers: [
    ScopeService
  ]
})
export class ScopeListComponent
  implements OnInit, OnDestroy, IMessageSubscriber<RefreshMessage> {
  private resource$: Subscription;
  private busSubscription: number;

  public searchText = '';
  public scopes: Array<Scope> = [];

  public constructor(
    private service: ScopeService,
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
    if (message.source === 'scope') {
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
    this.resource$ = this.service.scopes()
      .subscribe((response: Array<Scope>) => {
        this.scopes = response;
      });
  }
}
