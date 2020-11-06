import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';

import { AuthGuard } from '../shared';
import { SharedModule } from '../shared/shared.module';
import { FormdefModule } from '../shared/formdef/formdef.module';

import { ScopeService } from './services';
import { ScopeListComponent } from './components/list/scope-list.component';
import { ScopeDetailComponent } from './components/detail/scope-detail.component';
import { ScopeDashboardComponent } from './components/dashboard/scope-dashboard.component';

const ROUTES: Routes = [
  {
    path: 'scopes',
    component: ScopeDashboardComponent,
    canActivate: [AuthGuard],
    children: [
      {
        path: ':name',
        component: ScopeDetailComponent
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
    ScopeListComponent,
    ScopeDetailComponent,
    ScopeDashboardComponent
  ],
  providers: [
    ScopeService
  ]
})
export class ScopessModule { }
