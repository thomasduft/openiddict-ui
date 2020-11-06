import { Subscription } from 'rxjs';

import { Component, OnInit, OnDestroy } from '@angular/core';

import { AutoUnsubscribe, IMessageSubscriber, MessageBus } from '../../../shared';
import { RefreshMessage } from '../../../core';

import { Resource } from '../../models';
import { ResourceService } from '../../services/index';

@AutoUnsubscribe
@Component({
  selector: 'tw-resource-list',
  templateUrl: './resource-list.component.html',
  styleUrls: ['./resource-list.component.less'],
  providers: [
    ResourceService
  ]
})
export class ResourceListComponent
  implements OnInit, OnDestroy, IMessageSubscriber<RefreshMessage> {
  private resource$: Subscription;
  private busSubscription: number;

  public searchText = '';
  public resources: Array<Resource> = [];

  public constructor(
    private service: ResourceService,
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
    if (message.source === 'resource') {
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
    this.resource$ = this.service.resources()
      .subscribe((response: Array<Resource>) => {
        this.resources = response;
      });
  }
}
