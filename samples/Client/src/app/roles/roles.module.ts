import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';

import { AuthGuard } from '../shared';
import { SharedModule } from '../shared/shared.module';
import { FormdefModule } from '../shared/formdef/formdef.module';
import { FormdefRegistry } from '../shared/formdef';

import { RoleDetailSlot } from './models';
import { RoleService } from './services';
import { RoleDashboardComponent } from './components/dashboard/role-dashboard.component';
import { RoleListComponent } from './components/list/role-list.component';
import { RoleDetailComponent } from './components/detail/role-detail.component';

const ROUTES: Routes = [
  {
    path: 'roles',
    component: RoleDashboardComponent,
    canActivate: [AuthGuard],
    children: [
      {
        path: ':id',
        component: RoleDetailComponent
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
    RoleListComponent,
    RoleDetailComponent,
    RoleDashboardComponent
  ],
  providers: [
    RoleService
  ]
})
export class RolesModule {
  public constructor(
    private slotRegistry: FormdefRegistry
  ) {
    this.slotRegistry.register(new RoleDetailSlot());
  }
}
