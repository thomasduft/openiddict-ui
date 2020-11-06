import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SidebarModule } from './sidebar/sidebar.module';
import { WorkspaceModule } from './workspace/workspace.module';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    SidebarModule,
    WorkspaceModule
  ],
  exports: [
    SidebarModule,
    WorkspaceModule
  ]
})
export class ShellModule { }
