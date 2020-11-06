import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ExpanderComponent } from './expander.component';

@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [
    ExpanderComponent
  ],
  exports: [
    ExpanderComponent
  ]
})
export class ExpanderModule { }
