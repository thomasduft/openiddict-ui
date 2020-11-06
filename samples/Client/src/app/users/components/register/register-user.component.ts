import { Subscription } from 'rxjs';

import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import {
  AutoUnsubscribe,
  MessageBus,
  StatusMessage,
  StatusLevel,
  IdentityResult
} from '../../../shared';
import { RefreshMessage } from '../../../core';

import { RegisterUserSlot, RegisterUser } from '../../models';
import { AccountService } from '../../services';

@AutoUnsubscribe
@Component({
  selector: 'tw-register-user',
  templateUrl: './register-user.component.html',
  styleUrls: ['./register-user.component.less'],
  providers: [
    AccountService
  ]
})
export class RegisterUserComponent implements OnInit {
  private user$: Subscription;

  public key = RegisterUserSlot.KEY;
  public viewModel: RegisterUser;
  public errors: Array<string> = [];

  public constructor(
    private router: Router,
    private service: AccountService,
    private messageBus: MessageBus
  ) { }

  public ngOnInit(): void {
    this.key = RegisterUserSlot.KEY;
    this.viewModel = {
      userName: undefined,
      email: undefined,
      password: undefined,
      confirmPassword: undefined
    };
  }

  public submitted(viewModel: RegisterUser): void {
    viewModel.confirmPassword = viewModel.password;

    this.user$ = this.service.register(viewModel)
      .subscribe(() => {
        this.changesSaved();
        this.back();
      }, (error: IdentityResult) => {
        this.errors = error.errors;
      });
  }

  public back(): void {
    this.router.navigate(['users']);
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
