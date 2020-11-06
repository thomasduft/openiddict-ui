import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';

import { AuthGuard } from '../shared';
import { SharedModule } from '../shared/shared.module';
import { FormdefModule } from '../shared/formdef/formdef.module';
import { FormdefRegistry } from '../shared/formdef';

import { ClaimtypeDetailSlot } from './models';
import { ClaimTypesService } from './services';
import { ClaimtypeDashboardComponent } from './components/dashboard/claimtype-dashboard.component';
import { ClaimtypeListComponent } from './components/list/claimtype-list.component';
import { ClaimtypeDetailComponent } from './components/detail/claimtype-detail.component';

const ROUTES: Routes = [
  {
    path: 'claimtypes',
    component: ClaimtypeDashboardComponent,
    canActivate: [AuthGuard],
    children: [
      {
        path: ':id',
        component: ClaimtypeDetailComponent
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
    ClaimtypeListComponent,
    ClaimtypeDetailComponent,
    ClaimtypeDashboardComponent
  ],
  providers: [
    ClaimTypesService
  ]
})
export class ClaimtypesModule {
  public constructor(
    private slotRegistry: FormdefRegistry
  ) {
    this.slotRegistry.register(new ClaimtypeDetailSlot());
  }
}
