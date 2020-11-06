import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { SharedModule } from '../../shared/shared.module';
import { StatusModule } from './status/status.module';

import { WorkspaceComponent } from './workspace.component';

@NgModule({
  declarations: [WorkspaceComponent],
  imports: [
    CommonModule,
    RouterModule,
    SharedModule,
    StatusModule
  ],
  exports: [
    WorkspaceComponent
  ]
})
export class WorkspaceModule { }
