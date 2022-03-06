import { Subscription } from 'rxjs';

import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormGroup, Validators } from '@angular/forms';

import {
  AutoUnsubscribe,
  MessageBus,
  StatusMessage,
  StatusLevel,
  IdentityResult
} from '../../../shared';
import { FormdefRegistry, FormdefService } from '../../../shared/formdef';
import { RefreshMessage } from '../../../core';

import { RegisterApplicationSlot, RegisterApplication } from '../../models';
import { ApplicationService } from '../../services';

@AutoUnsubscribe
@Component({
  selector: 'tw-register-application',
  templateUrl: './register-application.component.html',
  providers: [
    ApplicationService,
    FormdefService
  ]
})
export class RegisterApplicationComponent implements OnInit {
  private registerApplication$: Subscription;

  public key = RegisterApplicationSlot.KEY;
  public viewModel: RegisterApplication;
  public errors: Array<string> = [];
  public form = new FormGroup({});

  public constructor(
    private router: Router,
    private service: ApplicationService,
    private messageBus: MessageBus,
    private slotRegistry: FormdefRegistry,
    private formdefService: FormdefService
  ) { }

  public ngOnInit(): void {
    const viewModel = {
      id: 'new',
      clientId: 'new',
      displayName: undefined,
      type: undefined,
      clientSecret: undefined,
      requirePkce: false,
      requireConsent: false
    };

    this.slotRegistry.register(new RegisterApplicationSlot());

    this.applyData(viewModel);
  }

  public submitted(viewModel: RegisterApplication): void {
    this.registerApplication$ = this.service.register(viewModel)
      .subscribe(() => {
        this.changesSaved();
        this.back();
      }, (error: IdentityResult) => {
        this.errors = error.errors;
      });
  }

  public back(): void {
    this.router.navigate(['applications']);
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

  private applyData(viewModel: RegisterApplication) {
    this.key = RegisterApplicationSlot.KEY;
    this.form = this.formdefService.toGroup(this.key, viewModel);
    this.viewModel = viewModel;

    this.applyFormBehavior(this.form);
  }

  private applyFormBehavior(form: FormGroup): void {
    const typeControl = form.get('type');
    if (typeControl) {
      typeControl.valueChanges.subscribe(type => {
        form.get('clientSecret').setValue(undefined);

        if (type === 'confidential') {
          form.get('clientSecret').enable();
          form.get('clientSecret').setValidators(Validators.required);

          form.get('requirePkce').disable();
          form.get('requirePkce').setValidators(null);
          form.get('requirePkce').setValue(false);
          form.get('requireConsent').disable();
          form.get('requireConsent').setValidators(null);
          form.get('requireConsent').setValue(false);
        } else {
          form.get('clientSecret').disable();
          form.get('clientSecret').setValidators(null);

          form.get('requirePkce').enable();
          form.get('requireConsent').enable();
        }

        form.get('clientSecret').updateValueAndValidity();
      });
    }
  }
}
