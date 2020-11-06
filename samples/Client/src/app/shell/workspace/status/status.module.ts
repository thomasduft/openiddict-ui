import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { IconModule } from '../../../shared/icon/icon.module';

import { StatusBarComponent } from './statusbar.component';

@NgModule({
  imports: [
    CommonModule,
    IconModule
  ],
  declarations: [
    StatusBarComponent
  ],
  exports: [
    StatusBarComponent
  ]
})
export class StatusModule { }
