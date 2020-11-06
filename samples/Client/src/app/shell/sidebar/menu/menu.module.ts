import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { IconModule } from './../../../shared/icon/icon.module';

import { MenuService } from './menu.service';
import { MenuComponent } from './menu.component';

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    IconModule
  ],
  declarations: [
    MenuComponent
  ],
  exports: [
    MenuComponent
  ],
  providers: [
    MenuService
  ]
})
export class MenuModule { }
