import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';

import { AuthGuard } from '../shared';
import { SharedModule } from '../shared/shared.module';
import { FormdefModule } from '../shared/formdef/formdef.module';

import { ResourceService } from './services';
import { ResourceListComponent } from './components/list/resource-list.component';
import { ResourceDetailComponent } from './components/detail/resource-detail.component';
import { ResourceDashboardComponent } from './components/dashboard/resource-dashboard.component';

const ROUTES: Routes = [
  {
    path: 'resources',
    component: ResourceDashboardComponent,
    canActivate: [AuthGuard],
    children: [
      {
        path: ':name',
        component: ResourceDetailComponent
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
    ResourceListComponent,
    ResourceDetailComponent,
    ResourceDashboardComponent
  ],
  providers: [
    ResourceService
  ]
})
export class ResourcesModule { }
