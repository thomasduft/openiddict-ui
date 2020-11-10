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

import { ClientDetailSlot, Client } from '../../models';
import { ClientService } from '../../services';

@AutoUnsubscribe
@Component({
  selector: 'tw-client-detail',
  templateUrl: './client-detail.component.html',
  styleUrls: ['./client-detail.component.less'],
  providers: [
    ClientService,
    ScopeService
  ]
})
export class ClientDetailComponent implements OnInit {
  private routeParams$: Subscription;
  private client$: Subscription;

  public key = ClientDetailSlot.KEY;
  public viewModel: Client;
  public errors: Array<string> = [];
  public isNew = false;

  public constructor(
    private router: Router,
    private route: ActivatedRoute,
    private service: ClientService,
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

  public submitted(viewModel: Client): void {
    if (this.isNew) {
      this.client$ = this.service.create(viewModel)
        .subscribe(
          () => this.handleSuccess(),
          (error: IdentityResult) => this.handleError(error)
        );
    } else {
      this.client$ = this.service.update(viewModel)
        .subscribe(
          () => this.handleSuccess(),
          (error: IdentityResult) => this.handleError(error)
        );
    }
  }

  public deleted(viewModel: Client): void {
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
          this.client$ = this.service.delete(viewModel.clientId)
            .subscribe((id: string) => {
              this.changesSaved();
              this.back();
            });
        }
      });
  }

  public back(): void {
    this.router.navigate(['clients']);
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
    this.client$ = forkJoin({
      scopenames: this.scopeService.scopenames(),
      client: this.service.client(id)
    }).subscribe((result: any) => {
      this.slotRegistry.register(new ClientDetailSlot(
        result.client.redirectUris,
        result.client.postLogoutRedirectUris
      ));

      this.key = ClientDetailSlot.KEY;
      this.viewModel = result.client;
    });
  }

  private create(): void {
    this.isNew = true;
    this.viewModel = {
      id: null,
      clientId: 'new',
      displayName: undefined,
      clientSecret: undefined,
      requirePkce: false,
      requireConsent: false,
      redirectUris: [],
      postLogoutRedirectUris: [],
      permissions: []
    };

    this.slotRegistry.register(new ClientDetailSlot(
      this.viewModel.redirectUris,
      this.viewModel.postLogoutRedirectUris
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

    this.messageBus.publish(new RefreshMessage('client'));
  }
}
