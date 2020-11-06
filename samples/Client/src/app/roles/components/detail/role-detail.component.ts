import { Subscription } from 'rxjs';

import { Component, OnInit, ElementRef } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import {
  AutoUnsubscribe,
  MessageBus,
  StatusMessage,
  StatusLevel,
  Popover,
  DeleteConfirmationComponent,
  PopoverCloseEvent,
  DeleteConfirmation
} from '../../../shared';
import { RefreshMessage } from '../../../core';

import { RoleDetailSlot, Role } from '../../models';
import { RoleService } from '../../services';

@AutoUnsubscribe
@Component({
  selector: 'tw-role-detail',
  templateUrl: './role-detail.component.html',
  styleUrls: ['./role-detail.component.less'],
  providers: [
    RoleService
  ]
})
export class RoleDetailComponent implements OnInit {
  private routeParams$: Subscription;
  private role$: Subscription;

  public key = RoleDetailSlot.KEY;
  public viewModel: Role;

  public isNew = false;

  public constructor(
    private router: Router,
    private route: ActivatedRoute,
    private service: RoleService,
    private popup: Popover,
    private element: ElementRef,
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

  public submitted(viewModel: Role): void {
    if (this.isNew) {
      this.role$ = this.service.create(viewModel)
        .subscribe((id: string) => {
          if (id) {
            this.changesSaved();
            this.back();
          }
        });
    } else {
      this.role$ = this.service.update(viewModel)
        .subscribe(() => {
          this.changesSaved();
          this.back();
        });
    }
  }

  public deleted(viewModel: Role): void {
    if (this.isNew) {
      return;
    }

    const origin = this.element.nativeElement;

    const popoverRef = this.popup
      .open<DeleteConfirmation>({
        content: DeleteConfirmationComponent,
        origin,
        hasBackdrop: false,
        data: {
          confirm: false,
          itemText: viewModel.name
        }
      });

    popoverRef.afterClosed$
      .subscribe((res: PopoverCloseEvent<DeleteConfirmation>) => {
        if (res.data.confirm) {
          this.role$ = this.service.delete(viewModel.id)
            .subscribe((id: string) => {
              this.changesSaved();
              this.back();
            });
        }
      });
  }

  public back(): void {
    this.router.navigate(['roles']);
  }

  private init(id?: string): void {
    if (id !== 'new') {
      this.load(id.toString());
    } else {
      this.create();
    }
  }

  private load(id: string): void {
    this.isNew = false;
    this.role$ = this.service.role(id)
      .subscribe((result: Role) => {
        this.key = RoleDetailSlot.KEY;
        this.viewModel = result;
      });
  }

  private create(): void {
    this.isNew = true;
    this.viewModel = {
      id: 'new',
      name: undefined
    };
  }

  private changesSaved(): void {
    this.messageBus.publish(
      new StatusMessage(
        undefined,
        'Your changes have been saved...',
        StatusLevel.Success
      ));

    this.messageBus.publish(new RefreshMessage('role'));
  }
}
