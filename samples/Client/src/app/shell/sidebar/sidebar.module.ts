import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { SharedModule } from '../../shared/shared.module';

import { MenuModule } from './menu/menu.module';

import { SidebarComponent } from './sidebar.component';

@NgModule({
  declarations: [SidebarComponent],
  imports: [
    CommonModule,
    RouterModule,
    SharedModule,
    MenuModule
  ],
  exports: [
    SidebarComponent
  ]
})
export class SidebarModule { }
