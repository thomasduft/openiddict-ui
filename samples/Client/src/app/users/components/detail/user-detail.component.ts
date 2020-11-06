import { Subscription } from 'rxjs';

import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { AutoUnsubscribe, MessageBus, StatusMessage, StatusLevel } from '../../../shared';
import { FormdefRegistry } from '../../../shared/formdef';
import { RefreshMessage } from '../../../core';

import { UserDetailSlot, User, UserDetail } from '../../models';
import { AccountService } from '../../services';

@AutoUnsubscribe
@Component({
  selector: 'tw-user-detail',
  templateUrl: './user-detail.component.html',
  styleUrls: ['./user-detail.component.less'],
  providers: [
    AccountService
  ]
})
export class UserDetailComponent implements OnInit {
  private routeParams$: Subscription;
  private user$: Subscription;

  public key = UserDetailSlot.KEY;
  public viewModel: User;

  public constructor(
    private router: Router,
    private route: ActivatedRoute,
    private service: AccountService,
    private slotRegistry: FormdefRegistry,
    private messageBus: MessageBus
  ) { }

  public ngOnInit(): void {
    this.routeParams$ = this.route.params
      .subscribe((params: Params) => {
        if (params.id) {
          this.init(params.id);
        }
      });
  }

  public submitted(viewModel: User): void {
    this.user$ = this.service.update(viewModel)
      .subscribe(() => {
        this.changesSaved();
        this.back();
      });
  }

  public deleted(viewModel: User): void {
    // TODO: delete user???
    this.changesSaved();
  }

  public back(): void {
    this.router.navigate(['users']);
  }

  private init(id?: string): void {
    this.load(id.toString());
  }

  private load(id: string): void {
    this.user$ = this.service.user(id)
      .subscribe((result: UserDetail) => {
        this.slotRegistry.register(new UserDetailSlot(
          result.claims,
          result.roles
        ));

        this.key = UserDetailSlot.KEY;
        this.viewModel = result.user;
      });
  }

  private changesSaved(): void {
    this.messageBus.publish(
      new StatusMessage(
        undefined,
        'Your changes have been saved...',
        StatusLevel.Success
      ));

    this.messageBus.publish(new RefreshMessage('user'));
  }
}
