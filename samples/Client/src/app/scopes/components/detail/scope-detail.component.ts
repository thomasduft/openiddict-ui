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
  DeleteConfirmation,
  IdentityResult
} from '../../../shared';
import { FormdefRegistry } from '../../../shared/formdef';
import { RefreshMessage } from '../../../core';

import { ScopeDetailSlot, Scope } from '../../models';
import { ScopeService } from '../../services';

@AutoUnsubscribe
@Component({
  selector: 'tw-scope-detail',
  templateUrl: './scope-detail.component.html',
  styleUrls: ['./scope-detail.component.less'],
  providers: [
    ScopeService
  ]
})
export class ScopeDetailComponent implements OnInit {
  private routeParams$: Subscription;
  private resource$: Subscription;

  public key = ScopeDetailSlot.KEY;
  public viewModel: Scope;
  public errors: Array<string> = [];
  public isNew = false;

  public constructor(
    private router: Router,
    private route: ActivatedRoute,
    private service: ScopeService,
    private slotRegistry: FormdefRegistry,
    private popup: Popover,
    private element: ElementRef,
    private messageBus: MessageBus
  ) { }

  public ngOnInit(): void {
    this.routeParams$ = this.route.params
      .subscribe((params: Params) => {
        if (params.name) {
          this.init(params.name);
        }
      });
  }

  public submitted(viewModel: Scope): void {
    if (this.isNew) {
      this.resource$ = this.service.create(viewModel)
        .subscribe(
          () => this.handleSuccess(),
          (error: IdentityResult) => this.handleError(error)
        );
    } else {
      this.resource$ = this.service.update(viewModel)
        .subscribe(
          () => this.handleSuccess(),
          (error: IdentityResult) => this.handleError(error)
        );
    }
  }

  public deleted(viewModel: Scope): void {
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
          this.resource$ = this.service.delete(viewModel.name)
            .subscribe((id: string) => {
              this.changesSaved();
              this.back();
            });
        }
      });
  }

  public back(): void {
    this.router.navigate(['scopes']);
  }

  private init(name?: string): void {
    if (name !== 'new') {
      this.load(name);
    } else {
      this.create();
    }
  }

  private load(name: string): void {
    this.isNew = false;
    this.resource$ = this.service.scope(name)
      .subscribe((result: Scope) => {
        this.slotRegistry.register(new ScopeDetailSlot());

        this.key = ScopeDetailSlot.KEY;
        this.viewModel = result;
      });
  }

  private create(): void {
    this.isNew = true;
    this.viewModel = {
      id: 0,
      enabled: true,
      name: 'new',
      displayName: undefined,
      description: undefined,
      required: false,
      showInDiscoveryDocument: false,
      emphasize: false,
      userClaims: []
    };
    this.slotRegistry.register(new ScopeDetailSlot());
  }

  private handleSuccess(): void {
    this.changesSaved();
    this.back();
  }

  private handleError(error: IdentityResult): void {
    this.errors = error.errors;
  }

  private changesSaved(): void {
    this.messageBus.publish(
      new StatusMessage(
        undefined,
        'Your changes have been saved...',
        StatusLevel.Success
      ));

    this.messageBus.publish(new RefreshMessage('scope'));
  }
}
