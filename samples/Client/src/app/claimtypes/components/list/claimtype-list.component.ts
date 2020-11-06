import { Subscription } from 'rxjs';

import { Component, OnInit, OnDestroy } from '@angular/core';

import { AutoUnsubscribe, IMessageSubscriber, MessageBus } from '../../../shared';
import { RefreshMessage } from '../../../core';

import { ClaimType } from '../../models';
import { ClaimTypesService } from '../../services/claimtypes.service';

@AutoUnsubscribe
@Component({
  selector: 'tw-claimtype-list',
  templateUrl: './claimtype-list.component.html',
  styleUrls: ['./claimtype-list.component.less'],
  providers: [
    ClaimTypesService
  ]
})
export class ClaimtypeListComponent
  implements OnInit, OnDestroy, IMessageSubscriber<RefreshMessage> {
  private claimtypes$: Subscription;
  private busSubscription: number;

  public searchText = '';
  public claimtypes: Array<ClaimType> = [];

  public constructor(
    private service: ClaimTypesService,
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
    if (message.source === 'claimtype') {
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
    this.loadClaimtypes();
  }

  private loadClaimtypes(): void {
    this.claimtypes$ = this.service.claimtypes()
      .subscribe((response: Array<ClaimType>) => {
        this.claimtypes = response;
      });
  }
}
