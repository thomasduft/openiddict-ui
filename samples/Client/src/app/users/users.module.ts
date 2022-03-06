import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';

import { AuthGuard } from '../shared';
import { SharedModule } from '../shared/shared.module';
import { FormdefModule } from '../shared/formdef/formdef.module';
import { FormdefRegistry } from '../shared/formdef';

import { AccountService } from './services';
import { UserDashboardComponent } from './components/dashboard/user-dashboard.component';
import { UserListComponent } from './components/list/user-list.component';
import { UserDetailComponent } from './components/detail/user-detail.component';
import { RegisterUserComponent } from './components/register/register-user.component';

import { RegisterUserSlot } from './models/register-user-slot.model';

const ROUTES: Routes = [
  {
    path: 'users',
    component: UserDashboardComponent,
    canActivate: [AuthGuard],
    children: [
      {
        path: 'register',
        component: RegisterUserComponent
      },
      {
        path: ':id',
        component: UserDetailComponent
      }
    ]
  }
];

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    RouterModule.forChild(ROUTES),
    SharedModule,
    FormdefModule
  ],
  declarations: [
    UserListComponent,
    UserDetailComponent,
    RegisterUserComponent,
    UserDashboardComponent
  ],
  providers: [
    AccountService
  ],
  exports: [
    UserDashboardComponent
  ]
})
export class UsersModule {
  public constructor(
    private slotRegistry: FormdefRegistry
  ) {
    this.slotRegistry.register(new RegisterUserSlot());
  }
}
