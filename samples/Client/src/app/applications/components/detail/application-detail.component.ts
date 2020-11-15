import { Subscription, forkJoin } from 'rxjs';

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
import { ScopeService } from '../../../scopes/services/scope.service';

import { ApplicationDetailSlot, Application } from '../../models';
import { ApplicationService } from '../../services';

@AutoUnsubscribe
@Component({
  selector: 'tw-application-detail',
  templateUrl: './application-detail.component.html',
  providers: [
    ApplicationService,
    ScopeService
  ]
})
export class ApplicationDetailComponent implements OnInit {
  private routeParams$: Subscription;
  private application$: Subscription;

  public key = ApplicationDetailSlot.KEY;
  public viewModel: Application;
  public errors: Array<string> = [];
  public isNew = false;

  public constructor(
    private router: Router,
    private route: ActivatedRoute,
    private service: ApplicationService,
    private scopeService: ScopeService,
    private slotRegistry: FormdefRegistry,
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

  public submitted(viewModel: Application): void {
    if (this.isNew) {
      this.application$ = this.service.create(viewModel)
        .subscribe(
          () => this.handleSuccess(),
          (error: IdentityResult) => this.handleError(error)
        );
    } else {
      this.application$ = this.service.update(viewModel)
        .subscribe(
          () => this.handleSuccess(),
          (error: IdentityResult) => this.handleError(error)
        );
    }
  }

  public deleted(viewModel: Application): void {
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
          itemText: viewModel.clientId
        }
      });

    popoverRef.afterClosed$
      .subscribe((res: PopoverCloseEvent<DeleteConfirmation>) => {
        if (res.data.confirm) {
          this.application$ = this.service.delete(viewModel.id)
            .subscribe((id: string) => {
              this.changesSaved();
              this.back();
            });
        }
      });
  }

  public back(): void {
    this.router.navigate(['applications']);
  }

  private init(id?: string): void {
    if (id !== 'new') {
      this.load(id);
    } else {
      this.create();
    }
  }

  private load(id: string): void {
    this.isNew = false;
    this.application$ = forkJoin({
      scopenames: this.scopeService.scopenames(),
      options: this.service.options(),
      application: this.service.application(id)
    }).subscribe((result: any) => {
      this.slotRegistry.register(new ApplicationDetailSlot(
        result.application.redirectUris,
        result.application.postLogoutRedirectUris,
        result.options.permissions,
        result.scopenames
      ));

      this.key = ApplicationDetailSlot.KEY;
      this.viewModel = result.application;
    });
  }

  private create(): void {
    this.isNew = true;
    this.viewModel = {
      id: 'new',
      clientId: 'new',
      displayName: undefined,
      clientSecret: undefined,
      requirePkce: false,
      requireConsent: false,
      redirectUris: [],
      postLogoutRedirectUris: [],
      permissions: []
    };

    this.slotRegistry.register(new ApplicationDetailSlot(
      this.viewModel.redirectUris,
      this.viewModel.postLogoutRedirectUris,
      this.viewModel.permissions,
      []
    ));
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

    this.messageBus.publish(new RefreshMessage('application'));
  }
}
