import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OverlayModule } from '@angular/cdk/overlay';

import { DeleteConfirmationComponent } from './delete-confirmation.component';
import { PopoverComponent } from './popover.component';

@NgModule({
  imports: [
    CommonModule,
    OverlayModule
  ],
  declarations: [
    DeleteConfirmationComponent,
    PopoverComponent
  ]
})
export class PopoverModule { }
