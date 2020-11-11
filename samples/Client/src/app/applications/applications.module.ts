import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';

import { AuthGuard } from '../shared';
import { SharedModule } from '../shared/shared.module';
import { FormdefModule } from '../shared/formdef/formdef.module';

import { ApplicationService } from './services';
import { ApplicationListComponent } from './components/list/application-list.component';
import { ApplicationDetailComponent } from './components/detail/application-detail.component';
import { ApplicationDashboardComponent } from './components/dashboard/application-dashboard.component';

const ROUTES: Routes = [
  {
    path: 'applications',
    component: ApplicationDashboardComponent,
    canActivate: [AuthGuard],
    children: [
      {
        path: ':id',
        component: ApplicationDetailComponent
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
    ApplicationListComponent,
    ApplicationDetailComponent,
    ApplicationDashboardComponent
  ],
  providers: [
    ApplicationService
  ]
})
export class ApplicationsModule { }
