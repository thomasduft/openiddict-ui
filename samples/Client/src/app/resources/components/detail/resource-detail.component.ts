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

import { ResourceDetailSlot, Resource } from '../../models';
import { ResourceService } from '../../services';

@AutoUnsubscribe
@Component({
  selector: 'tw-resource-detail',
  templateUrl: './resource-detail.component.html',
  styleUrls: ['./resource-detail.component.less'],
  providers: [
    ResourceService,
    ScopeService
  ]
})
export class ResourceDetailComponent implements OnInit {
  private routeParams$: Subscription;
  private resource$: Subscription;
  private scopes$: Subscription;

  public key = ResourceDetailSlot.KEY;
  public viewModel: Resource;
  public errors: Array<string> = [];
  public isNew = false;

  public constructor(
    private router: Router,
    private route: ActivatedRoute,
    private service: ResourceService,
    private scopeService: ScopeService,
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

  public submitted(viewModel: Resource): void {
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

  public deleted(viewModel: Resource): void {
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
    this.router.navigate(['resources']);
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
    this.resource$ = forkJoin({
      scopenames: this.scopeService.scopenames(),
      resource: this.service.resource(name)
    }).subscribe((result: any) => {
      this.slotRegistry.register(new ResourceDetailSlot(
        result.scopenames,
        result.resource.userClaims
      ));

      this.key = ResourceDetailSlot.KEY;
      this.viewModel = result.resource;
    });
  }

  private create(): void {
    this.isNew = true;
    this.scopes$ = this.scopeService.scopenames()
      .subscribe((scopes: Array<string>) => {
        this.viewModel = {
          id: 0,
          enabled: true,
          name: 'new',
          displayName: undefined,
          scopes: [],
          userClaims: []
        };
        this.slotRegistry.register(new ResourceDetailSlot(
          scopes,
          this.viewModel.userClaims
        ));
      });
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

    this.messageBus.publish(new RefreshMessage('resource'));
  }
}
