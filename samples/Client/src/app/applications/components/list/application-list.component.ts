import { Subscription } from 'rxjs';

import { Component, OnInit, OnDestroy } from '@angular/core';

import { AutoUnsubscribe, IMessageSubscriber, MessageBus } from '../../../shared';
import { RefreshMessage } from '../../../core';

import { Application } from '../../models';
import { ApplicationService } from '../../services/index';

@AutoUnsubscribe
@Component({
  selector: 'tw-application-list',
  templateUrl: './application-list.component.html',
  providers: [
    ApplicationService
  ]
})
export class ApplicationListComponent
  implements OnInit, OnDestroy, IMessageSubscriber<RefreshMessage> {
  private applications$: Subscription;
  private busSubscription: number;

  public searchText = '';
  public applications: Array<Application> = [];

  public constructor(
    private service: ApplicationService,
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
    this.applications$ = this.service.applications()
      .subscribe((response: Array<Application>) => {
        this.applications = response;
      });
  }
}
