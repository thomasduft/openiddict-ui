import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';

import { AuthGuard } from '../shared';
import { SharedModule } from '../shared/shared.module';
import { FormdefModule } from '../shared/formdef/formdef.module';

import { ClientService } from './services';
import { ClientListComponent } from './components/list/client-list.component';
import { ClientDetailComponent } from './components/detail/client-detail.component';
import { ClientDashboardComponent } from './components/dashboard/client-dashboard.component';

const ROUTES: Routes = [
  {
    path: 'clients',
    component: ClientDashboardComponent,
    canActivate: [AuthGuard],
    children: [
      {
        path: ':clientId',
        component: ClientDetailComponent
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
    ClientListComponent,
    ClientDetailComponent,
    ClientDashboardComponent
  ],
  providers: [
    ClientService
  ]
})
export class ClientsModule { }
